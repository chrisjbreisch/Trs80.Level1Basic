10 input "Horizontal address (0 to 127)"; x
12 in. "The starting vertical address #(0 to 47) is";v
14 in. "How many vertical blocks do you wish to fill"; a
16 if v+1<48 goto 20
18 print "Too many vertical blocks. Would wrap-around!"
19 end
20 cls
30 for y = v to v+a
40   set(x,y)
50 next y
99 goto 99