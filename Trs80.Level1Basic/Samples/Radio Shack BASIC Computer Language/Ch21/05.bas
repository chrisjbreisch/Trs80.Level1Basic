10 in. "Which car's engine & color do you want to know";w
50 for l = 1 to 10
55  read a(l)
60 next l
80 for s=101 to 110
90   read a(s)
90 next s
100 data 300,200,500,300,200
110 data 300,400,400,300,500
170 print
180 p. "LICENSE #", "ENGINE SIZE", "COLOR CODE"
210 print w, a(w), a(w+100)
300 data 3,1,4,3,2,4,3,2,1,3