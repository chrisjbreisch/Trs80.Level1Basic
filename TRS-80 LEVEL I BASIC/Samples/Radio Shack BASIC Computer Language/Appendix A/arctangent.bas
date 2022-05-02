30000 end
30660 rem * arctangent * input x, output c, a
30670 rem c is in degrees, a is in radians
30680 rem also uses b, t internally
30683 rem depends upon sign.bas
30685 rem unit tests added to original. Use: run 32000
30690 gos. 30810 : x=abs(x) : c=0
30700 if x>1 t. c=1 : x=1/x
30710 a=x*x
30720 b=((2.86623e-3*a-1.61657e-2)*a+4.29096e-2)*a
30730 b=((((b-7.5289e-2)*a+.106563)*a-.142089)*a+.199936)*a
30740 a=((b-.333332)*a+1)*x
30750 if c=1 t. a=1.570796-a
30760 a=t*a : c=a*57.29578 : ret.


31999 end
32000 cls
32005 x=10000 : gosub 30690
32010 p."ARCTAN(10000)--Expected: [89.9942 1.5707], Actual:[";c;a;"]"
32015 x=1000 : gosub 30690
32020 p."ARCTAN(1000)--Expected: [89.9427 1.5698], Actual:[";c;a;"]"
32025 x=100 : gosub 30690
32030 p."ARCTAN(100)--Expected: [89.4271 1.5608], Actual:[";c;a;"]"
32035 x=10 : gosub 30690
32040 p."ARCTAN(10)--Expected: [84.2894 1.47113], Actual:[";c;a;"]"
32045 x=1 : gosub 30690
32050 p."ARCTAN(1)--Expected: [45 0.785398], Actual:[";c;a;"]"
32055 x=0 : gosub 30690
32060 p."ARCTAN(0)--Expected: [0 0], Actual:[";c;a;"]"
32065 x=-1 : gosub 30690
32070 p."ARCTAN(-1)--Expected: [-45 -0.785398], Actual:[";c;a;"]"
32075 x=-10 : gosub 30690
32080 p."ARCTAN(-10)--Expected: [-84.2894 -1.47113], Actual:[";c;a;"]"
32085 x=-100 : gosub 30690
32090 p."ARCTAN(-100)--Expected: [-89.4271 -1.5608], Actual:[";c;a;"]"
32095 x=-1000 : gosub 30690
32100 p."ARCTAN(-1000)--Expected: [-89.9427 -1.5698], Actual:[";c;a;"]"
32105 x=-10000 : gosub 30690
32110 p."ARCTAN(10000)--Expected: [-89.9942 -1.5707], Actual:[";c;a;"]"
