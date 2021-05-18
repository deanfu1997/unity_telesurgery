clear all
close all
clc
%% import data
dean(1,1) =  "user_study/dean_MTM_p_1.txt";
dean(1,2) =  "user_study/dean_MTM_p_2.txt";
dean(1,3) =  "user_study/dean_MTM_p_3.txt";


dean(2,1) =  "user_study/dean_IMU_p_1.txt";
dean(2,2) =  "user_study/dean_IMU_p_2.txt";
dean(2,3) =  "user_study/dean_IMU_p_3.txt";


ehsan(1,1) =  "user_study/ehsan_MTM_p_1.txt";
ehsan(1,2) =  "user_study/ehsan_MTM_p_2.txt";
ehsan(1,3) =  "user_study/ehsan_MTM_p_3.txt";

ehsan(2,1) =  "user_study/ehsan_IMU_p_1.txt";
ehsan(2,2) =  "user_study/ehsan_IMU_p_2.txt";
ehsan(2,3) =  "user_study/ehsan_IMU_p_3.txt";

%%
t1 = zeros(2,4);
t2 = zeros(2,4);
p1 = zeros(2,4);
p2 = zeros(2,4);
o1 = zeros(2,4);
o2 = zeros(2,4);

t11 = zeros(2,8);
p11 = zeros(2,8);
o11 = zeros(2,8);

k = 1;
%% 
disp("Dean's piece");
figure(k)
k = k+1;
sgtitle('Piece-wise Wire Task, User 1')
color = ['r', 'g', 'b'];
i = 1;

pos_error = 0;
ori_error = 0;
pos_error_imu = 0;
ori_error_imu = 0;

pbox_easy = [];
obox_easy = [];
pbox_easy_IMU = [];
obox_easy_IMU = [];

pbox_easy_label = [];
obox_easy_label = [];
pbox_easy_label_IMU = [];
obox_easy_label_IMU = [];

pbox_hard = [];
obox_hard = [];
pbox_hard_IMU = [];
obox_hard_IMU = [];

pbox_hard_label = [];
obox_hard_label = [];
pbox_hard_label_IMU = [];
obox_hard_label_IMU = [];

for j = 1:3
    filename_dean = dean(1,j);
    filename_dean_IMU = dean(2,j);
    data_dean = importdata(filename_dean);
    data_dean_IMU = importdata(filename_dean_IMU);
    timeStart_dean = data_dean(1,1);
    timeStart_dean_IMU = data_dean_IMU(1,1);
    timeC = data_dean(end,1) - timeStart_dean;
    timeC_IMU = data_dean_IMU(end,1) - timeStart_dean_IMU;
    xd = data_dean(:,1) - timeStart_dean;
    xd_IMU = data_dean_IMU(:,1) - timeStart_dean_IMU;
    dd = data_dean(:,2)*1000;
    od = data_dean(:,3);
    dd_IMU = data_dean_IMU(:,2)*1000;
    od_IMU = data_dean_IMU(:,3);
    pos_error = mean(dd);
    ori_error = mean(od);
    pos_error_imu = mean(dd_IMU);
    ori_error_imu = mean(od_IMU);

    pbox_easy = [pbox_easy;dd];
    obox_easy = [obox_easy;od];
    pbox_easy_IMU = [pbox_easy_IMU;dd_IMU];
    obox_easy_IMU = [obox_easy_IMU;od_IMU];

    
    t1(1,j) = timeC;
    t1(2,j) = timeC_IMU;
    p1(1,j) = pos_error;
    p1(2,j) = pos_error_imu;
    o1(1,j) = ori_error;
    o1(2,j) = ori_error_imu;
   
    % Plot starts here
    subplot1 = subplot(2,2,1);
    hold on
    
    
    plot(xd,dd,color(i))
    title('position error of MTM')
    xlabel('time(s)');
    ylabel('position error (mm)');
    ylim(subplot1,[0 55])
    
    subplot2 = subplot(2,2,2);
    hold on
    
    
    plot(xd_IMU,dd_IMU,color(i))
    title('position error of IMU system')
    xlabel('time(s)');
    ylabel('position error (mm)');
    ylim(subplot2,[0 55])

    subplot3 = subplot(2,2,3);
    hold on
    
    plot(xd,od,color(i))
    title('orientation error of MTM')
    xlabel('time(s)');
    ylabel('orientation error (deg)')
    ylim(subplot3,[0 60])
    
    subplot4 = subplot(2,2,4);
    hold on
    
    plot(xd_IMU,od_IMU,color(i))
    title('orientation error of IMU system')
    xlabel('time (s)');
    ylabel('orientation error (deg)')
    ylim(subplot4,[0 60])
    i = i+1;
end

filename_dean = dean(1,1);
filename_dean_IMU = dean(2,1);
data_dean = importdata(filename_dean);
data_dean_IMU = importdata(filename_dean_IMU);
timeStart_dean = data_dean(1,1);
timeStart_dean_IMU = data_dean_IMU(1,1);
timeC = data_dean(end,1) - timeStart_dean;
timeC_IMU = data_dean_IMU(end,1) - timeStart_dean_IMU;
xd = data_dean(:,1) - timeStart_dean;
xd_IMU = data_dean_IMU(:,1) - timeStart_dean_IMU;
subplot1 = subplot(2,2,1);
plot(xd,17.5*ones(size(xd)),'k--')
subplot2 = subplot(2,2,2);
plot(xd_IMU,17.5*ones(size(xd_IMU)),'k--')
legend(subplot1, 'Trial 1','Trial 2','Trial 3','Collision')
legend(subplot2, 'Trial 1','Trial 2','Trial 3','Collision')
legend(subplot3, 'Trial 1','Trial 2','Trial 3')
legend(subplot4, 'Trial 1','Trial 2','Trial 3')

pbox_easy_label = repmat({'1'},size(pbox_easy,1),1);
obox_easy_label = repmat({'1'},size(obox_easy,1),1);
pbox_easy_label_IMU = repmat({'2'},size(pbox_easy_IMU,1),1);
obox_easy_label_IMU = repmat({'2'},size(obox_easy_IMU,1),1);


%%
% figure(k)
% k = k+1;
% sgtitle('S-shaped Wire Task, User 1')
% i = 1;
% disp("Dean's S_shaped");
% for j = 4:6
%     filename_dean = dean(1,j);
%     filename_dean_IMU = dean(2,j);
%     data_dean = importdata(filename_dean);
%     data_dean_IMU = importdata(filename_dean_IMU);
%     timeStart_dean = data_dean(1,1);
%     timeStart_dean_IMU = data_dean_IMU(1,1);
%     xd = data_dean(:,1) - timeStart_dean;
%     xd_IMU = data_dean_IMU(:,1) - timeStart_dean_IMU;
%     timeC = data_dean(end,1) - timeStart_dean;
%     timeC_IMU = data_dean_IMU(end,1) - timeStart_dean_IMU;
% 
%     dd = data_dean(:,2)*1000;
%     od = data_dean(:,3);
%     dd_IMU = data_dean_IMU(:,2)*1000;
%     od_IMU = data_dean_IMU(:,3);
%     pos_error = mean(dd);
%     ori_error = mean(od);
%     pos_error_imu = mean(dd_IMU);
%     ori_error_imu = mean(od_IMU) ;
%     
%     pbox_hard = [pbox_hard;dd];
%     obox_hard = [obox_hard;od];
%     pbox_hard_IMU = [pbox_hard_IMU;dd_IMU];
%     obox_hard_IMU = [obox_hard_IMU;od_IMU];
% 
% 
%     
%     t1(1,j) = timeC;
%     t1(2,j) = timeC_IMU;
%     p1(1,j) = pos_error;
%     p1(2,j) = pos_error_imu;
%     o1(1,j) = ori_error;
%     o1(2,j) = ori_error_imu;
%     
%     subplot1 = subplot(2,2,1);
%     hold on
%     
%     plot(xd,dd,color(i))
%     title('position error of MTM')
%     xlabel('time(s)');
%     ylabel('position error (mm)');
%     ylim(subplot1,[0 70])
%     
%     subplot2 = subplot(2,2,2);
%     hold on
%     
%     plot(xd_IMU,dd_IMU,color(i))
%     title('position error of IMU system')
%     xlabel('time(s)');
%     ylabel('position error (mm)');
%     ylim(subplot2,[0 70])
% 
%     subplot3 = subplot(2,2,3);
%     hold on
%     
%     plot(xd,od,color(i))
%     title('orientation error of MTM')
%     xlabel('time(s)');
%     ylabel('orientation error (deg)')
%     ylim(subplot3,[0 70])
%     
%     subplot4 = subplot(2,2,4);
%     hold on
%     
%     plot(xd_IMU,od_IMU,color(i))
%     title('orientation error of IMU system')
%     xlabel('time(s)');
%     ylabel('orientation error (deg)')
%     ylim(subplot4,[0 70])
%     i = i+1;
% end
% legend(subplot1, 'Trial 1','Trial 2','Trial 3')
% legend(subplot2, 'Trial 1','Trial 2','Trial 3')
% legend(subplot3, 'Trial 1','Trial 2','Trial 3')
% legend(subplot4, 'Trial 1','Trial 2','Trial 3')
% 
% pbox_hard_label = repmat({'1'},size(pbox_hard,1),1);
% obox_hard_label = repmat({'1'},size(obox_hard,1),1);
% pbox_hard_label_IMU = repmat({'2'},size(pbox_hard_IMU,1),1);
% obox_hard_label_IMU = repmat({'2'},size(obox_hard_IMU,1),1);


%% boxplot variables
epbox_easy = [];
eobox_easy = [];
epbox_easy_IMU = [];
eobox_easy_IMU = [];

epbox_easy_label = [];
eobox_easy_label = [];
epbox_easy_label_IMU = [];
eobox_easy_label_IMU = [];

epbox_hard = [];
eobox_hard = [];
epbox_hard_IMU = [];
eobox_hard_IMU = [];

epbox_hard_label = [];
eobox_hard_label = [];
epbox_hard_label_IMU = [];
eobox_hard_label_IMU = [];

%%
figure(k)
k = k+1;
sgtitle('Piece-wise Wire Task, User 2')
color = ['r', 'g', 'b'];
i = 1;
disp("Ehsan's piece");
for j = 1:3
   
    filename_ehsan = ehsan(1,j);
    
    filename_ehsan_IMU = ehsan(2,j);

    data_ehsan = importdata(filename_ehsan);
    
    data_ehsan_IMU = importdata(filename_ehsan_IMU);
    
    timeStart_ehsan = data_ehsan(1,1);
    
    timeStart_ehsan_IMU = data_ehsan_IMU(1,1);
    
    timeC = data_ehsan(end,1) - timeStart_ehsan;
    timeC_IMU = data_ehsan_IMU(end,1) - timeStart_ehsan_IMU;

    

    xe = data_ehsan(:,1) - timeStart_ehsan;
    
    xe_IMU = data_ehsan_IMU(:,1) - timeStart_ehsan_IMU;
    
    de = data_ehsan(:,2)*1000;
    
    oe = data_ehsan(:,3);
    
    de_IMU = data_ehsan_IMU(:,2)*1000;
    
    oe_IMU = data_ehsan_IMU(:,3);
    
    pos_error = mean(de);
    ori_error = mean(oe);
    pos_error_imu = mean(de_IMU);
    ori_error_imu = mean(oe_IMU);
    
    epbox_easy = [epbox_easy;de];
    eobox_easy = [eobox_easy;oe];
    epbox_easy_IMU = [epbox_easy_IMU;de_IMU];
    eobox_easy_IMU = [eobox_easy_IMU;oe_IMU];

    
    t2(1,j) = timeC;
    t2(2,j) = timeC_IMU;
    p2(1,j) = pos_error;
    p2(2,j) = pos_error_imu;
    o2(1,j) = ori_error;
    o2(2,j) = ori_error_imu;
    subplot1 = subplot(2,2,1);
    hold on
    
    plot(xe,17.5*ones(size(xe)),'k--')
    plot(xe,de,color(i))
    title('position error of MTM')
    xlabel('time(s)');
    ylabel('position error (mm)');
    ylim(subplot1,[0 55])
    
    subplot2 = subplot(2,2,2);
    hold on
    
    plot(xe_IMU,17.5*ones(size(xe_IMU)),'k--')
    plot(xe_IMU,de_IMU,color(i))
    title('position error of IMU system')
    xlabel('time(s)');
    ylabel('position error (mm)');
    ylim(subplot2,[0 55])

    subplot3 = subplot(2,2,3);
    hold on
    
    plot(xe,oe,color(i))
    title('orientation error of MTM')
    xlabel('time(s)');
    ylabel('orientation error (deg)')
    ylim(subplot3,[0 60])
    
    subplot4 = subplot(2,2,4);
    hold on
    
    plot(xe_IMU,oe_IMU,color(i))
    title('orientation error of IMU system')
    xlabel('time(s)');
    ylabel('orientation error (deg)')
    ylim(subplot4,[0 60])
    i = i+1;
end
legend(subplot1, 'Collision','Trial 1','Trial 2','Trial 3')
legend(subplot2, 'Collision','Trial 1','Trial 2','Trial 3')
legend(subplot3, 'Trial 1','Trial 2','Trial 3')
legend(subplot4, 'Trial 1','Trial 2','Trial 3')

epbox_easy_label = repmat({'3'},size(epbox_easy,1),1);
eobox_easy_label = repmat({'3'},size(eobox_easy,1),1);
epbox_easy_label_IMU = repmat({'4'},size(epbox_easy_IMU,1),1);
eobox_easy_label_IMU = repmat({'4'},size(eobox_easy_IMU,1),1);
    

%% 
% figure(k)
% k = k+1;
% sgtitle('S-shaped Wire Task, User 2')
% i = 1;
% disp("Ehsan's s-shape");
% for j = 4:6
%     
%     filename_ehsan = ehsan(1,j);
%     
%     filename_ehsan_IMU = ehsan(2,j);
%     
%     data_ehsan = importdata(filename_ehsan);
%     
%     data_ehsan_IMU = importdata(filename_ehsan_IMU);
%     
%     timeStart_ehsan = data_ehsan(1,1);
%     
%     timeStart_ehsan_IMU = data_ehsan_IMU(1,1);
%     timeC = data_ehsan(end,1) - timeStart_ehsan;
%     timeC_IMU = data_ehsan_IMU(end,1) - timeStart_ehsan_IMU;
% 
%     xe = data_ehsan(:,1) - timeStart_ehsan;
% 
%     xe_IMU = data_ehsan_IMU(:,1) - timeStart_ehsan_IMU;
% 
%     de = data_ehsan(:,2)*1000;
% 
%     oe = data_ehsan(:,3);
% 
%     de_IMU = data_ehsan_IMU(:,2)*1000;
% 
%     oe_IMU = data_ehsan_IMU(:,3);
%     pos_error = mean(de);
%     ori_error = mean(oe);
%     pos_error_imu = mean(de_IMU);
%     ori_error_imu = mean(oe_IMU); 
%     
%     epbox_hard = [pbox_hard;de];
%     eobox_hard = [obox_hard;oe];
%     epbox_hard_IMU = [pbox_hard_IMU;de_IMU];
%     eobox_hard_IMU = [obox_hard_IMU;oe_IMU];
% 
%     
%     t2(1,j) = timeC;
%     t2(2,j) = timeC_IMU;
%     p2(1,j) = pos_error;
%     p2(2,j) = pos_error_imu;
%     o2(1,j) = ori_error;
%     o2(2,j) = ori_error_imu;
%     subplot1 = subplot(2,2,1);
%     hold on
%     
%     plot(xe,de,color(i))
%     title('position error of MTM')
%     xlabel('time(s)');
%     ylabel('position error (mm)');
%     ylim(subplot1,[0 70])
%     
%     subplot2 = subplot(2,2,2);
%     hold on
%     
%     plot(xe_IMU,de_IMU,color(i))
%     title('position error of IMU system')
%     xlabel('time(s)');
%     ylabel('position error (mm)');
%     ylim(subplot2,[0 70])
% 
%     subplot3 = subplot(2,2,3);
%     hold on
%     
%     plot(xe,oe,color(i))
%     title('orientation error of MTM')
%     xlabel('time(s)');
%     ylabel('orientation error (deg)')
%     ylim(subplot3,[0 70])
%     
%     subplot4 = subplot(2,2,4);
%     hold on
%     
%     plot(xe_IMU,oe_IMU,color(i))
%     title('orientation error of IMU system')
%     xlabel('time(s)');
%     ylabel('orientation error (deg)')
%     ylim(subplot4,[0 70])
%     i = i+1;
% end
% legend(subplot1, 'Trial 1','Trial 2','Trial 3')
% legend(subplot2, 'Trial 1','Trial 2','Trial 3')
% legend(subplot3, 'Trial 1','Trial 2','Trial 3')
% legend(subplot4, 'Trial 1','Trial 2','Trial 3')
% 
% epbox_hard_label = repmat({'3'},size(epbox_hard,1),1);
% eobox_hard_label = repmat({'3'},size(eobox_hard,1),1);
% epbox_hard_label_IMU = repmat({'4'},size(epbox_hard_IMU,1),1);
% eobox_hard_label_IMU = repmat({'4'},size(eobox_hard_IMU,1),1);

%% Calculate mean across number of trials
for i = 1:2
    t1(i,4) = mean(t1(i,1:3));
    t2(i,4) = mean(t2(i,1:3));
    
    p1(i,4) = mean(p1(i,1:3));

    p2(i,4) = mean(p2(i,1:3));
    
    o1(i,4) = mean(o1(i,1:3));

    o2(i,4) = mean(o2(i,1:3));

    
end

%% round to 3 significant digits
t1 = round(t1,3,'significant');
t2 = round(t2,3,'significant');
p1 = round(p1,3,'significant');
p2 = round(p2,3,'significant');
o1 = round(o1,3,'significant');
o2 = round(o2,3,'significant');

%% Convert data into desired table format
for i = 1:2
    t11(i,1:3) = t1(i,1:3);
    t11(i,5:7) = t2(i,1:3);
    t11(i,4) = t1(i,4);
    t11(i,8) = t2(i,4);
    
    p11(i,1:3) = p1(i,1:3);
    p11(i,5:7) = p2(i,1:3);
    p11(i,4) = p1(i,4);
    p11(i,8) = p2(i,4);
    
    
    o11(i,1:3) = o1(i,1:3);
    o11(i,5:7) = o2(i,1:3);
    o11(i,4) = o1(i,4);
    o11(i,8) = o2(i,4);
    
end

figure(k)
k = k+1;
sgtitle("Overall Box Plot")
subplot1 = subplot(1,2,1);
boxplot([pbox_easy; pbox_easy_IMU; epbox_easy; epbox_easy_IMU],[pbox_easy_label; pbox_easy_label_IMU; epbox_easy_label; epbox_easy_label_IMU],'symbol','')
h = findobj(gca,'Tag','Median');
set(h,'Visible','off');
hold on
plot(1,p11(1,4), 'k*')
plot(2,p11(2,4), 'k*')
plot(3,p11(1,8), 'k*')
plot(4,p11(2,8), 'k*')

title('position error of Piece-wise Wire Task')
ylabel('position error (mm)');
subplot3 = subplot(1,2,2);
boxplot([obox_easy; obox_easy_IMU; eobox_easy; eobox_easy_IMU],[obox_easy_label; obox_easy_label_IMU; eobox_easy_label; eobox_easy_label_IMU],'symbol','')
h = findobj(gca,'Tag','Median');
set(h,'Visible','off');
hold on
plot(1,o11(1,4), 'k*')
plot(2,o11(2,4), 'k*')
plot(3,o11(1,8), 'k*')
plot(4,o11(2,8), 'k*')
title('orientation error of Piece-wise Wire Task')
ylabel('orientation error (deg)')

ylim(subplot1,[0 35])
ylim(subplot2,[0 35])
ylim(subplot3,[0 60])
ylim(subplot4,[0 60])

leg = ['1: User1 MTM','2: User1 IMU','3: User2 MTM','4: User2 IMU'];


t11 %completion time

p11 %position error
o11 %orientation error


%% Error precentage

% position MTM User 1
tot = size(pbox_easy,1);
suc = pbox_easy(pbox_easy<17.5);
perM1 = size(suc,1)/tot

% position MTM User 2
tot = size(epbox_easy,1);
suc = epbox_easy(epbox_easy<17.5);
perM2 = size(suc,1)/tot

% position IMU User 1
tot = size(pbox_easy_IMU,1);
suc = pbox_easy_IMU(pbox_easy_IMU<17.5);
perI1 = size(suc,1)/tot

% position IMU User 2
tot = size(epbox_easy_IMU,1);
suc = epbox_easy_IMU(epbox_easy_IMU<17.5);
perI2 = size(suc,1)/tot
