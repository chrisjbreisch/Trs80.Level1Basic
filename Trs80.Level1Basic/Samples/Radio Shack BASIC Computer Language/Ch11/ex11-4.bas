10 r = .01
20 d = 1
30 t = .01
35 cls
40 print "DAY ", "DAILY ", "TOTAL "
50 print "  #", "RATE ", "EARNED "
60 print
70 print d, r, t
80 if r > 1e6 end
90 r = r * 2
100 d = d + 1
110 t = t + r
120 goto 70