1 IN. "ENTER A NUMBER FROM 1 TO 100";N
2 F.I=1TON;J=RND(32767);N.I
10 REM * CRAPS GAME *
20 CLS
30 GOSUB 300:P=N
40 P.:P."YOU ROLLED ****";A;" AND ";B;"****"
50 ON P GOTO 60, 120, 120, 100, 100, 100, 110, 100, 100, 100, 110, 120
60 REM * USED FOR THE ON STATEMENT IF P=1 (WHICH IT CAN'T)*
100 P."YOUR POINT IS";N:GOTO 130
110 PRINT "YOU WIN!":P.:END
120 PRINT "YOU LOSE.":P.:END
130 GOSUB 300:M=N
135 P.:P."YOU ROLLED ****";A;" AND ";B;"****"
140 IF P=M THEN 110
150 IF M=7 THEN 120
160 G.130
300 A=RND(6):B=RND(6):N=A+B
310 RETURN
