10 REM * DEMONSTRATION OF GRAPHICS 'POINT' STATEMENT *
20 P=15:L=119
30 CLS
40 P.AT5,"THIS IS A DEMONSTRATION OF THE POINT STATEMENT---";
50 P.AT56,"X   Y";
100 F.I=1TO P:SET(RND(113),RND(45)+2):N.I
110 F.X=0 TO 111:F.Y=0 TO 47
120    IF POINT(X,Y) = 0 GOTO 160
130    P.AT L,X;:P.AT L+4,Y;
140    L=L+64
150    G.170
160    SET(X,Y):RESET(X,Y)
170 N.Y:N.X
180 P.AT4,"THE COORDINATES OF THE GRAPHICS BLOCK ARE >>-->>";
190 P.AT 0;
200 G.200