10 CLS
20 FOR M=0 TO 127
30    SET(M,0)
40    SET(M,47)
50 NEXT M
60 Y = 14
70 D = 1
80 RESET (64, Y+48-D)
90 SET(64,Y)
100 Y = Y+D
105 IF Y=46 THEN 180
110 IF Y=48 THEN 130
115 IF Y=1 THEN 180
120 IF Y<>-1 THEN 80
130 Y=Y-2*D
140 D=-D
150 PRINT AT Y*64+32, "    "
160 G. 90
180 PRINT AT Y*64+32, "PING"
190 G. 90
999 GOTO 999