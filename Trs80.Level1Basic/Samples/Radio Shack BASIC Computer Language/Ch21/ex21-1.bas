10 IN. "WHICH CAR'S ENGINE, COLOR, & STYLE DO YOU WANT TO KNOW";W
50 FOR L = 1 TO 10
55  READ A(L)
60 NEXT L
80 FOR S=101 TO 110
85   READ A(S)
90 NEXT S
100 DATA 300,200,500,300,200
110 DATA 300,400,400,300,500
130 FOR B = 201 TO 210
133   READ A(B)
140 NEXT B
170 PRINT
180 P. "LICENSE #", "ENGINE SIZE", "COLOR CODE", "BODY STYLE"
210 PRINT W, A(W), A(W+100), A(W+200)
300 DATA 3,1,4,3,2,4,3,2,1,3
400 DATA 20,20,10,20,30,20,30,10,20,20