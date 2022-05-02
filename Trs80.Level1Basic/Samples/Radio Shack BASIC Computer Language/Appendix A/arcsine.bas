30000 end
30530 rem * arcsin subroutine * input s, output y, w
30535 rem y is in degrees, w is in radians
30540 rem also uses variables x,z internally
30542 rem differs from original. Fixed /0 error when s=1
30543 rem fixed rounding error on 30580
30544 rem fixed typo on 30550
30545 rem Unit tests added to original. Use: run 32000
30550 x=s : if abs(s)<=.707107 t. 30610
30555 w=1 : if s<0 t. w=-1
30560 x=1-s*s : if x<0 t. p. s;"is out of range" : stop
30565 if x=0 t. y=w*90 : w=w*1.5708 : ret.
30570 w=x/2 : z=0
30580 y=(x/w-w)/2 : if (abs(y)<1e-3)*(y=z) t. x=w : g. 30610
30600 w=w+y : z = y : g. 30580
30610 y=x+x*x*x/6+x*x*x*x*x*.075+x*x*x*x*x*x*x*4.464286e-2
30620 w=y+x*x*x*x*x*x*x*x*x*3.038194e-2
30625 if abs(s)>.707107 t. w=1.570796-w
30630 y=w*57.29578 : ret.


31999 end
32000 cls
32105 s=1 : gosub 30550
32110 p."ARCSIN(1)--Expected: [90 1.5708], Actual:[";y;w;"]"
32115 s=0.707107 : gosub 30550
32120 p."ARCSIN(0.707107)--Expected: [45 0.785398], Actual:[";y;w;"]"
32125 s=0.5 : gosub 30550
32130 p."ARCSIN(0.5)--Expected: [30 0.523599], Actual:[";y;w;"]"
32135 s=0.25 : gosub 30550
32140 p."ARCSIN(0.25)--Expected: [14.4775 0.25268], Actual:[";y;w;"]"
32155 s=0 : gosub 30550
32160 p."ARCSIN(0)--Expected: [0 0], Actual:[";y;w;"]"
32165 s=-0.25 : gosub 30550
32170 p."ARCSIN(-0.25)--Expected: [-14.4775 -0.25268], Actual:[";y;w;"]"
32175 s=-0.5 : gosub 30550
32180 p."ARCSIN(-0.5)--Expected: [-30 -0.523599], Actual:[";y;w;"]"
32185 s=-0.707107 : gosub 30550
32190 p."ARCSIN(-0.707107)--Expected: [-45 -0.785398], Actual:[";y;w;"]"
32195 s=-1 : gosub 30550
32200 p."ARCSIN(-1)--Expected: [-90 -1.5708], Actual:[";y;w;"]"
