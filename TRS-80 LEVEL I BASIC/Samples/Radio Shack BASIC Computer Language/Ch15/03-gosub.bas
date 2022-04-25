10 input "type any number";x
20 gosub 30800
30 on t+2 goto 50, 60, 70
45 end
50 print "The number is negative."
55 end
60 print "The number is zero."
65 end
70 print "The number is positive."
75 end
30000 end
30800 rem * sgn(x) input x, output t = -1, 0, 1
30810 if x < 0 then t = -1
30820 if x = 0 then t = 0
30830 if x > 0 then t = 1
30840 return