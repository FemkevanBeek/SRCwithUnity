clear all; close all; clc;
%%Process Simulink and Unity data for Soft Robotic setup (based on Teleoperation code) 
%%Femke van Beek, 06-10-2021

%Simulink_desired_file = 'Desired_pressure4';
%Simulink_measured_file = 'Received_pressure4';
Unity_file = 'SR_finger_ramp_yesFF2021-01-21-09-01-48.txt';
%Unity_file = 'Testfile_Unity2021-31-19-06-31-03_FF_on.txt';
%Unity_file = 'Testfile_Unity2021-13-20-04-13-00.txt';
%Unity_file = 'Testfile_Unity2021-13-20-04-13-00.txt';

startframe_Unity = 1;
%startframe_Simulink = 1;

%Des_Simulink = load(Simulink_desired_file);
%Meas_Simulink = load(Simulink_measured_file);
Des_and_meas_Unity = csvread(Unity_file,1,0);

Unity_freq = mean(1./diff(Des_and_meas_Unity(startframe_Unity:end,1)));
[corr,lags] = xcorr(Des_and_meas_Unity(startframe_Unity:end,3),Des_and_meas_Unity(startframe_Unity:end,4),100);
[dum,ind]=max(corr);
Unity_lag = lags(ind);

figure()
plot(Des_and_meas_Unity(startframe_Unity:end,1),Des_and_meas_Unity(startframe_Unity:end,3),'b^');
hold on
plot(Des_and_meas_Unity(startframe_Unity:end,1),Des_and_meas_Unity(startframe_Unity:end,4),'g.');
legend({'Desired pressure [kPa]','Measured pressure [kPa]'});
xlabel('Time [s]');
ylabel('Pressure [kPa]');
title(['Unity data: frequency = ',num2str(round(Unity_freq)), ' Hz; lag = ',num2str(Unity_lag), ' frames']);


return
Simulink_freq = mean(1./diff(Des_Simulink.ans.Time(startframe_Simulink:end)));
[corr_S,lags_S] = xcorr(Des_Simulink.ans.Time(startframe_Simulink:end),Meas_Simulink.ans.Time(startframe_Simulink:end),100);
[dum_S,ind_S]=max(corr_S);
Simulink_lag = lags(ind_S);

figure()
plot(Des_Simulink.ans.Time(startframe_Simulink:end),Des_Simulink.ans.Data(startframe_Simulink:end).*10,'b--');
hold on
plot(Meas_Simulink.ans.Time(startframe_Simulink:end),Meas_Simulink.ans.Data(startframe_Simulink:end),'g:');
legend({'Desired pressure [kPa]','Measured pressure [kPa]'});
xlabel('Time [s]');
ylabel('Pressure [kPa]');
title(['Simulink data: frequency = ',num2str(round(Simulink_freq)), ' Hz; lag = ',num2str(Simulink_lag), ' frames']);