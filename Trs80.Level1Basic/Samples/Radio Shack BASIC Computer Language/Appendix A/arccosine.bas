30000 end
30500 rem * arccosine * input x, output y, w
30510 rem y is in degrees, w is in radians
30515 rem depends upon arcsine.bas
30517 rem Added unit tests to original. Use: run 32000
30520 gos. 30550 : y=90-y : w = 1.570796 - w : ret.

31999 end
32000 cls
32005 s=1 : gosub 30520
32010 p."ARCCOS(1)--Expected: [0 0], Actual:[";y;w;"]"
32015 s=0.707107 : gosub 30520
32020 p."ARCCOS(0.707107)--Expected: [45 0.785398], Actual:[";y;w;"]"
32025 s=0.5 : gosub 30520
32030 p."ARCCOS(0.5)--Expected: [60 1.0472], Actual:[";y;w;"]"
32035 s=0.25 : gosub 30520
32040 p."ARCCOS(0.25)--Expected: [75.5225 1.31812], Actual:[";y;w;"]"
32055 s=0 : gosub 30520
32060 p."ARCCOS(0)--Expected: [90 1.5708], Actual:[";y;w;"]"
32065 s=-0.25 : gosub 30520
32070 p."ARCCOS(-0.25)--Expected: [104.4775 1.82348], Actual:[";y;w;"]"
32075 s=-0.5 : gosub 30520
32080 p."ARCCOS(-0.5)--Expected: [120 2.0944], Actual:[";y;w;"]"
32085 s=-0.707107 : gosub 30520
32090 p."ARCCOS(-0.707107)--Expected: [135 2.3562], Actual:[";y;w;"]"
32095 s=-1 : gosub 30520
32100 p."ARCCOS(-1)--Expected: [180 3.14159], Actual:[";y;w;"]"
