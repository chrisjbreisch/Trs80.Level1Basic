30000 end
30350 rem * cosine * input x in degrees, output y
30351 rem also uses w,z internally
30352 rem depends upon sin.bas
30355 rem Added unit tests to original. Use: run 32000
30360 w=1:if x<0 t. w=-1
30362 x=x+90:gos.30376:if(z=-1)*(w=1)t.y=-y
30365 ret.


31999 end
32000 cls
32105 x=0 : gosub 30360
32110 p."COSINE(0)--Expected: 1, Actual:";y
32115 x=45 : gosub 30360
32120 p."COSINE(45)--Expected: 0.707107, Actual:";y
32125 x=90 : gosub 30360
32130 p."COSINE(90)--Expected: 0, Actual:";y
32135 x=135 : gosub 30360
32140 p."COSINE(135)--Expected: -0.707107, Actual:";y
32145 x=180 : gosub 30360
32150 p."COSINE(180)--Expected: -1, Actual:";y
32155 x=225 : gosub 30360
32160 p."COSINE(225)--Expected: -.707107, Actual:";y
32165 x=270 : gosub 30360
32170 p."COSINE(270)--Expected: 0, Actual:";y
32175 x=315 : gosub 30360
32180 p."COSINE(315)--Expected: 0.707107, Actual:";y
32185 x=22.5 : gosub 30360
32190 p."COSINE(22.5)--Expected: 0.92388, Actual:";y
