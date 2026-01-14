matrix = randn(10, 10) * 5;
vector = [1 2 3 4 5]';
carr = 'It is a char array!';
noise = rand(1,100) - rand(1, 100);
sinus = 10 * sin(linspace(0, 2*pi, 100)) + noise;
complex = rand(1, 5) * (10 + 10i);
save('test_hdf5.mat', '-v7.3');