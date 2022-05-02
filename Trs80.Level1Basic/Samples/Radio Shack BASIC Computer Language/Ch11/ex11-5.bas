1 rem * find the largest area *
5 cls
10 print "WIRE FENCE", "LENGTH ", "WIDTH ", "AREA "
20 print " (FEET)", " (FEET)", " (FEET)", " (SQ. FEET)"
30 f = 1000
40 for l = 0 to 500 step 50
50   w = (f-2*l)/2
60   a = l * w
70   print f,l,w,a
80 next l
90 end