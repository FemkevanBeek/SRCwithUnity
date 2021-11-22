clear all; close all; clc;

t = [0:0.01:3.01];
amp = [1:0.01:4.01];
f = [1:0.02:7.02];
y = amp .* sin (f .* 2 .* pi .* t);

figure()
plot(t,y);