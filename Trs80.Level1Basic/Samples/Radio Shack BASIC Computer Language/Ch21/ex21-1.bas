10 in. "Which car's engine, color, & style do you want to know";w
50 for l = 1 to 10
55  read a(l)
60 next l
80 for s=101 to 110
90   read a(s)
90 next s
100 data 300,200,500,300,200
110 data 300,400,400,300,500
130 for b = 201 to 210
133   read a(b)
140 next b
170 print
180 p. "LICENSE #", "ENGINE SIZE", "COLOR CODE", "BODY STYLE"
210 print w, a(w), a(w+100), a(w+200)
300 data 3,1,4,3,2,4,3,2,1,3
400 data 20,20,10,20,30,20,30,10,20,20