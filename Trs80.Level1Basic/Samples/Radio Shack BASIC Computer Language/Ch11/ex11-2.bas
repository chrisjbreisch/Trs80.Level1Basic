10 REM * TIME VS. RATE FLIGHT CHART *
20 CLS
30 D = 3000
40 PRINT "    B O S T O N   T O   S A N   D I E G O "
50 PRINT
60 PRINT "RATE", "TIME","DISTANCE"
65 PRINT "(MPH)", "(HOURS)","(MILES)"
70 PRINT
80 FOR R = 200 TO 1000 STEP 100
85 IF R <> 600 THEN 90
87 FOR X = 1 TO 2000000
88 NEXT X
90 T = D / R
100 PRINT R, T, D
110 NEXT R