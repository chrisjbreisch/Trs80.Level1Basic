10 rem * time vs. rate flight chart *
20 cls
30 d = 3000
40 print "    B O S T O N   T O   S A N   D I E G O "
50 print
60 print "RATE (MPH)", "TIME (HOURS)","DISTANCE (MILES)"
70 print
80 for r = 200 to 1000 step 100
90 t = d / r
100 print r, t, d
110 next r