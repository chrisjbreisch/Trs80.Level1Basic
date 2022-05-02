30000 end
30800 rem * sign * input x, output t=-1,0,+1
30805 rem Unit tests added to original. Use: run 32000
30810 if x < 0 t. t=-1
30820 if x = 0 t. t=0
30830 if x > 0 t. t=1
30840 ret.


31999 end
32000 cls
32155 x=0 : gosub 30800
32160 p."SIGN(0)--Expected: 0, Actual:";t
32165 x=4 : gosub 30800
32170 p."SIGN(4)--Expected: 1, Actual:";t
32175 x=-4 : gosub 30800
32180 p."SIGN(-4)--Expected: -1, Actual:";t
32185 x=32768 : gosub 30800
32190 p."SIGN(32768)--Expected: 1, Actual:";t
32195 x=1000000 : gosub 30800
32200 p."SIGN(1000000)--Expected: 1, Actual:";t
32205 x=-32768 : gosub 30800
32210 p."SIGN(-32768)--Expected: -1, Actual:";t
32215 x=-1000000 : gosub 30800
32220 p."SIGN(-1000000)--Expected: -1, Actual:";t
