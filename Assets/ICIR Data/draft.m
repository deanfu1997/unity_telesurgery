epbox_easy_1 = [];
eobox_easy_1 = [];
epbox_easy_IMU_1 = [];
eobox_easy_IMU_1 = [];

epbox_easy_label_1 = [];
eobox_easy_label_1 = [];
epbox_easy_label_IMU_1 = [];
eobox_easy_label_IMU_1 = [];

epbox_hard_1 = [];
eobox_hard_1 = [];
epbox_hard_IMU_1 = [];
eobox_hard_IMU_1 = [];

epbox_hard_label_1 = [];
eobox_hard_label_1 = [];
epbox_hard_label_IMU_1 = [];
eobox_hard_label_IMU_1 = [];

%%

color = ['r', 'g', 'b'];
i = 1;
disp("Ehsan's straight");
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
    
    epbox_easy_1 = [epbox_easy_1;de];
    eobox_easy_1 = [eobox_easy_1;oe];
    epbox_easy_IMU_1 = [epbox_easy_IMU_1;de_IMU];
    eobox_easy_IMU_1 = [eobox_easy_IMU_1;oe_IMU];

    
    t2(1,j) = timeC;
    t2(2,j) = timeC_IMU;
    p2(1,j) = pos_error;
    p2(2,j) = pos_error_imu;
    o2(1,j) = ori_error;
    o2(2,j) = ori_error_imu;
    subplot1 = subplot(2,2,1);
    hold on
    
    
    i = i+1;
end


epbox_easy_label_1 = repmat({'3'},size(epbox_easy_1,1),1);
eobox_easy_label_1 = repmat({'3'},size(eobox_easy_1,1),1);
epbox_easy_label_IMU_1 = repmat({'4'},size(epbox_easy_IMU_1,1),1);
eobox_easy_label_IMU_1 = repmat({'4'},size(eobox_easy_IMU_1,1),1);
    

%% 

i = 1;
disp("Ehsan's s-shape");
for j = 4:6
    
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
    
    epbox_hard_1 = [pbox_hard_1;de];
    eobox_hard_1 = [obox_hard_1;oe];
    epbox_hard_IMU_1 = [pbox_hard_IMU_1;de_IMU];
    eobox_hard_IMU_1 = [obox_hard_IMU_1;oe_IMU];

    
    t2(1,j) = timeC;
    t2(2,j) = timeC_IMU;
    p2(1,j) = pos_error;
    p2(2,j) = pos_error_imu;
    o2(1,j) = ori_error;
    o2(2,j) = ori_error_imu;
    
    i = i+1;
end


epbox_hard_label_1 = repmat({'3'},size(epbox_hard_1,1),1);
eobox_hard_label_1 = repmat({'3'},size(eobox_hard_1,1),1);
epbox_hard_label_IMU_1 = repmat({'4'},size(epbox_hard_IMU_1,1),1);
eobox_hard_label_IMU_1 = repmat({'4'},size(eobox_hard_IMU_1,1),1);

%% Calculate mean across number of trials
for i = 1:2
    t1(i,7) = mean(t1(i,1:3));
    t1(i,8) = mean(t1(i,4:6));
    t2(i,7) = mean(t2(i,1:3));
    t2(i,8) = mean(t2(i,4:6));
    p1(i,7) = mean(p1(i,1:3));
    p1(i,8) = mean(p1(i,4:6));
    p2(i,7) = mean(p2(i,1:3));
    p2(i,8) = mean(p2(i,4:6));
    o1(i,7) = mean(o1(i,1:3));
    o1(i,8) = mean(o1(i,4:6));
    o2(i,7) = mean(o2(i,1:3));
    o2(i,8) = mean(o2(i,4:6));
    
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
    t11(i,4) = t1(i,7);
    t11(i,8) = t2(i,7);
    
    t22(i,1:3) = t1(i,4:6);
    t22(i,5:7) = t2(i,4:6);
    t22(i,4) = t1(i,8);
    t22(i,8) = t2(i,8);
    
    p11(i,1:3) = p1(i,1:3);
    p11(i,5:7) = p2(i,1:3);
    p11(i,4) = p1(i,7);
    p11(i,8) = p2(i,7);
    
    p22(i,1:3) = p1(i,4:6);
    p22(i,5:7) = p2(i,4:6);
    p22(i,4) = p1(i,8);
    p22(i,8) = p2(i,8);
    
    o11(i,1:3) = o1(i,1:3);
    o11(i,5:7) = o2(i,1:3);
    o11(i,4) = o1(i,7);
    o11(i,8) = o2(i,7);
    
    o22(i,1:3) = o1(i,4:6);
    o22(i,5:7) = o2(i,4:6);
    o22(i,4) = o1(i,8);
    o22(i,8) = o2(i,8);
end