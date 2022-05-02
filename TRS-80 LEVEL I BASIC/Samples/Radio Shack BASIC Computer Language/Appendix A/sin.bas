30000 end
30370 rem * sin * input x in degrees, output y
30371 rem also uses z internally
30373 rem Added unit tests to original. Use: run 32000
30376 z=1 : if x<0 t. x=-x : z=-1
30380 if x>360 t. x=x/360 : x=(x-int(x))*360
30390 if x>90t.x=x/90:y=int(x):x=(x-y)*90:onyg.30410,30420,30430
30400 x=x/57.29578 : if abs(x)<2.48616e-4 y=0: ret.
30405 g.30440
30410 x=90-x : g.30400
30420 x=-x : g.30400
30430 x=x-90 : g.30400
30440 y=x-x*x*x/6+x*x*x*x*x/120-x*x*x*x*x*x*x/5040
30450 y=y+x*x*x*x*x*x*x*x*x/362880 : if z=-1t. y=-y
30455 ret.

31999 end
32000 cls
32005 x=0 : gosub 30376
32010 p."SIN(0)--Expected: 0, Actual:";y
32015 x=45 : gosub 30376
32020 p."SIN(45)--Expected: 0.707107, Actual:";y
32025 x=90 : gosub 30376
32030 p."SIN(90)--Expected: 1, Actual:";y
32035 x=135 : gosub 30376
32040 p."SIN(135)--Expected: 0.707107, Actual:";y
32045 x=180 : gosub 30376
32050 p."SIN(180)--Expected: 0, Actual:";y
32055 x=225 : gosub 30376
32060 p."SIN(225)--Expected: -.707107, Actual:";y
32065 x=270 : gosub 30376
32070 p."SIN(270)--Expected: -1, Actual:";y
32075 x=315 : gosub 30376
32080 p."SIN(315)--Expected: -.707107, Actual:";y
32085 x=22.5 : gosub 30376
32090 p."SIN(22.5)--Expected: 0.382683, Actual:";y
