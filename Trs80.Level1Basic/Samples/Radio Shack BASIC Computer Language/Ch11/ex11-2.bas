10 rem * time vs. rate flight chart *
20 cls
30 d = 3000
40 print "    B O S T O N   T O   S A N   D I E G O "
50 print
60 print "RATE", "TIME","DISTANCE"
65 print "(MPH)", "(HOURS)","(MILES)"
70 print
80 for r = 200 to 1000 step 100
85 if r <> 600 then 90
87 for x = 1 to 2000000
88 next x
90 t = d / r
100 print r, t, d
110 next r