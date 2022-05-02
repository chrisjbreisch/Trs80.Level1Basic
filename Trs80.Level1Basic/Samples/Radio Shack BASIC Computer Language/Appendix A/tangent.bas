30000 end
30300 rem * tangent * input x in degrees, output y
30310 rem also uses a,c,w,z internally
30315 rem depends upon sin.bas & cosine.bas
30317 rem Unit tests added to original. Use: run 32000
30320 a=x : gos. 30360
30330 if abs(y)<1e-5 t. p. "Tangent undefined": stop
30340 c=y : x=a : gos.30376 : y=y/c : ret.

31999 end
32000 cls
32205 x=0 : gosub 30320
32210 p."TAN(0)--Expected: 0, Actual:";y
32215 x=45 : gosub 30320
32220 p."TAN(45)--Expected: 1, Actual:";y
32225 x=89 : gosub 30320
32230 p."TAN(89)--Expected: 57.29, Actual:";y
32235 x=91 : gosub 30320
32237 p."TAN(89)--Expected: -57.29, Actual:";y
32238 x=135 : gosub 30320
32240 p."TAN(135)--Expected: -1, Actual:";y
32245 x=180 : gosub 30320
32250 p."TAN(180)--Expected: 0, Actual:";y
32255 x=225 : gosub 30320
32260 p."TAN(225)--Expected: 1, Actual:";y
32265 x=269 : gosub 30320
32270 p."TAN(269)--Expected: 57.29, Actual:";y
32275 x=271 : gosub 30320
32270 p."TAN(271)--Expected: -57.29, Actual:";y
32275 x=315 : gosub 30320
32280 p."TAN(315)--Expected: -1, Actual:";y
32285 x=22.5 : gosub 30320
32290 p."TAN(22.5)--Expected: 0.414214, Actual:";y
