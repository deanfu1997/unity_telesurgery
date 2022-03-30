% CIS2 project
% Guanhao(Dean) Fu
% gfu3@jhu.edu

clear all
close all
clc

syms lu lf wp
FK = cell(2,2);

wrist2palm = [wp 0 -0.026 1]'; % might need to change
trail = importdata("calibration/calibration.txt");

if (size(trail,1) == 48)
    add = [lu lf];
    for i = 0:11
        bl{i+1} = trail(1+i*4:4+i*4,:);
%         bl{i+1}% for debug
        bl{i+1} = sym(bl(i+1));

        if (mod(i,3) == 0)
            continue
        else
            bl{i+1}(1,4) = add(mod(i,3));
        end
    end  
    for j = 1:4
        FK{j} = bl{1+(j-1)*3}*bl{2+(j-1)*3}*bl{3+(j-1)*3}*wrist2palm;
        % FK{j}    
    end
    
else
    disp('calibration data wrong, please check calibration.txt');
end

% actual distance between calibration posts
length = 0.205; %in meters
width = 0.146; %in meters
diag = sqrt(length^2+width^2);

d = [length, diag, width, length, width, diag, diag, width, length, width, diag, length];
objsum = 0;
for i = 1:4
    idx = [1 ,2, 3, 4];
    idx(find(idx == i)) = [];
    for j = 1:3
        e = abs(FK{i}-FK{idx(j)});
        e = e(1:3);
        e = norm(e);
        d((i-1)*3+j);
        obj = norm((e-d((i-1)*3+j)),1);
        objsum = objsum + obj;
    end
end

ht = matlabFunction(objsum)

%% change this according to ht
fun = @(x)abs(sqrt(abs(x(2).*1.305487041e-1+x(1).*1.3557e-1+conj(x(3)).*1.32802360254915e-1+3.165389538611534e-3).^2+abs(x(2).*2.293819788e-1+x(1).*4.3489e-1+conj(x(3)).*2.36495960637534e-1+1.003502265126468e-2).^2+abs(x(2).*2.05819488e-2+x(1).*2.4187e-1+conj(x(3)).*2.1873679463922e-2+1.827083829573964e-3).^2)-2.516763795035203e-1).*2.0+abs(sqrt(abs(x(2).*1.419083256e-1-x(1).*7.505e-2+conj(x(3)).*1.39200519461679e-1-3.873624282688342e-3).^2+abs(x(2).*6.6554749e-3-x(1).*4.762e-2+conj(x(3)).*1.0120674480488e-2+4.912773857104204e-3).^2+abs(x(2).*7.82805204e-2+x(1).*5.9503e-1+conj(x(3)).*8.901460235967e-2+1.520462582405276e-2).^2)-4.1e+1./2.0e+2).*2.0+abs(sqrt(abs(x(2).*2.724570297e-1+x(1).*6.052e-2+conj(x(3)).*2.72002879716594e-1-7.08234744076808e-4).^2+abs(x(2).*2.72374237e-2+x(1).*1.9425e-1+conj(x(3)).*3.199435394441e-2+6.739857686678168e-3).^2+abs(x(2).*1.511014584e-1-x(1).*1.6014e-1+conj(x(3)).*1.47481358277864e-1-5.16960317278808e-3).^2)-7.3e+1./5.0e+2).*2.0+abs(sqrt(abs(x(2).*4.96584205e-2+x(1).*5.9603e-1+conj(x(3)).*6.0414815117579e-2+1.524300971456897e-2).^2+abs(x(2).*5.25958473e-2+x(1).*1.6412e-1+conj(x(3)).*5.9491380750126e-2+9.766850159690208e-3).^2+abs(x(2).*3.800052872e-1-x(1).*1.996e-2+conj(x(3)).*3.76336289942977e-1-5.29285272235265e-3).^2)-2.516763795035203e-1).*2.0+abs(sqrt(abs(x(2).*2.53584236e-2-x(1).*3.013e-2+conj(x(3)).*2.7497026805716e-2+3.02699247301204e-3).^2+abs(x(2).*(-1.075482575e-1)+x(1).*8.048e-2-conj(x(3)).*1.04333410226383e-1+4.584617978275842e-3).^2+abs(x(2).*2.007598789e-1+x(1).*4.3589e-1+conj(x(3)).*2.07896173395443e-1+1.007340654178089e-2).^2)-4.1e+1./2.0e+2).*2.0+abs(sqrt(abs(x(2).*2.86220999e-2-x(1)./1.0e+3+conj(x(3)).*2.8599787242091e-2-3.8383890516218e-5).^2+abs(x(2).*2.380969616e-1+x(1).*5.509e-2+conj(x(3)).*2.37135770481298e-1-1.419228439664308e-3).^2+abs(x(2).*4.59403724e-2+x(1).*2.1174e-1+conj(x(3)).*4.9370706269638e-2+4.854076302586004e-3).^2)-7.3e+1./5.0e+2).*2.0

% Run fmincon
lb = [0,0,0]; % lower bound
ub = [1,1,1]; % upper bound
A = [];
b = [];
Aeq = [];
beq = [];
% lu ground truths is 0.278m, lf ground is 0.259m, lh ground is 0.18
x0 = [0.278,0.259,0.18];
x = fmincon(fun,x0,A,b,Aeq,beq,lb,ub)

%%

lu_acc = [3.057553957
0.935251799
24.56834532
14.35251799
21.65467626
1.618705036
0.647482014
2.050359712
9.100719424
18.66906475
];

lf_acc = [11.31274131
4.671814672
7.683397683
15.52123552
6.023166023
11.54440154
4.401544402
0.347490347
0.193050193
16.44787645];

figure(1)
hold on
plot(lu_acc,'r','LineWidth',2)
plot(lf_acc,'b','LineWidth',2)
plot(lu_acc,'ro')
plot(lf_acc,'bo')

hold off
xlabel("Trial")
ylabel("Error Percentage")
title("Calibration error")
xlim([1,10])
ylim([0,26])
legend(["upper arm length error","lower arm length error"])

%%
lu_acc
lf_acc
lu_avg = mean(lu_acc)
lf_avg = mean(lf_acc)
lu_std = std(lu_acc)
lf_std = std(lf_acc)


