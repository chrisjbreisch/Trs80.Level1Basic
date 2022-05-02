10 input "Type any number";x
20 rem * sgn routine *
22 if x < 0 then t = -1
24 if x = 0 then t = 0
26 if x > 0 then t = 1
30 on t+2 goto 50, 60, 70
45 end
50 print "The number is negative."
55 end
60 print "The number is zero."
65 end
70 print "The number is positive."