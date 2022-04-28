2 input "How many seconds delay do you wish";s
3 p = 2000000
4 d = s * p
5 for x = 1 to d
6 next x
7 print "Delay is over. Took";s;" seconds."
9 end
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