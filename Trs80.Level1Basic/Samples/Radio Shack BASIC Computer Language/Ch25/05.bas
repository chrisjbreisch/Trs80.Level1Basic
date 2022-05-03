10 IN. "HOW FAR ARE YOU FROM BASE OF TREE";D
20 IN. "WHAT IS ANGLE BETWEEN TIP AND BASE OF TREE";A
30 X=A:GOSUB 30320
40 H=INT(D*Y+.5)
50 IF H=28 THEN 80
60 P. "FIND ANOTHER TREE--THIS ONE IS";H;"FEET TALL."
70 P.:GOTO 10
80 P. "CHOP IT DOWN AND TAKE IT HOME!"

30000 END
30300 REM * TANGENT * INPUT X IN DEGREES, OUTPUT Y
30310 REM ALSO USES A,C,W,Z INTERNALLY
30320 A=X : GOS. 30360
30330 IF ABS(Y)<1E-5 T. P. "TANGENT UNDEFINED": STOP
30340 C=Y : X=A : GOS.30376 : Y=Y/C : RET.

30000 END
30350 REM * COSINE * INPUT X IN DEGREES, OUTPUT Y
30351 REM ALSO USES W,Z INTERNALLY
30360 W=ABS(X)/X:X=X+90:GOS.30376:IF(Z=-1)*(W=1)T.Y=-Y
30365 RET.

30000 END
30370 REM * SIN * INPUT X IN DEGREES, OUTPUT Y
30371 REM ALSO USES Z INTERNALLY
30376 Z=ABS(X)/X:X=Z*X
30380 IF X>360 T. X=X/360 : X=(X-INT(X))*360
30390 IF X>90T.X=X/90:Y=INT(X):X=(X-Y)*90:ONYG.30410,30420,30430
30400 X=X/57.29578 : IF ABS(X)<2.48616E-4 Y=0: RET.
30405 G.30440
30410 X=90-X : G.30400
30420 X=-X : G.30400
30430 X=X-90 : G.30400
30440 Y=X-X*X*X/6+X*X*X*X/120-X*X*X*X*X*X/5040
30450 Y=Y+X*X*X*X*X*X*X*X*X/362880 : IF Z=-1T. Y=-Y
30455 RET.