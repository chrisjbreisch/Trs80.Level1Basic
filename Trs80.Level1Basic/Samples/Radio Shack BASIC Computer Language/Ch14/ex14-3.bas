10 P=3.14159
20 PRINT "RADIUS", "AREA"
30 PRINT
40 FOR R=1 TO 10
50   A = P * R * R
55   A = INT(A*100)/100
60   PRINT R, A
70 NEXT R