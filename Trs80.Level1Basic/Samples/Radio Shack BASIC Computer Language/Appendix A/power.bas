30000 end
30100 rem * power * input x,y: output p
30110 rem also uses e,l,a,b,c internally
30115 rem depends upon logarithm.bas and exp.bas
30117 rem Added unit tests to original. Use: run 32000
30120 p=1 : e=0 : if y = 0 t. ret.
30130 if (x<0)*(int(y)=y) t. p=1-2*y+4*int(y/2) : x=-x
30140 if x<>0 t. gos. 30190 : x=y*l : gos. 30250
30150 p=p*e : ret.

31999 end
32000 cls
32005 x=1 : y = 1 : gosub 30100
32010 p."POW(1, 1)--Expected: 1, Actual:";p
32015 x=2 : y = 2 : gosub 30100
32020 p."POW(2, 2)--Expected: 4, Actual:";p
32025 x=3 : y = 3 : gosub 30100
32030 p."POW(3, 3)--Expected: 27, Actual:";p
32035 x=4 : y = 4 : gosub 30100
32040 p."POW(4, 4)--Expected: 256, Actual:";p
32045 x=2 : y = 10 : gosub 30100
32050 p."POW(2, 10)--Expected: 1024, Actual:";p
32055 x=2 : y = -1 : gosub 30100
32060 p."POW(2, -1)--Expected: 0.5, Actual:";p
32065 x=3 : y = 3.5 : gosub 30100
32070 p."POW(3, 3.5)--Expected: 46.7654, Actual:";p
32075 x=4 : y = 0.5 : gosub 30100
32080 p."POW(4, 0.5)--Expected: 2, Actual:";p
32085 x=27 : y = 0.333333 : gosub 30100
32090 p."POW(27, 0.333333)--Expected: 3, Actual:";p
