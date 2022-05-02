10 rem * test grader *
20 cls
30 p. "This is a test grading program"
40 p. "Enter the student's five answers as requested"
50 restore
60 n=0
70 for i=1 to 5
80    print "Answer number";i;
90    input a
100   read b
110   print a,b;
120   if a=b then print "Correct";:n=n+1
130   print
140 next i
150 print n;"right out of 5";
160 print n/5*100;"% "
170 p. "Any more tests to grade";
180 in."  --1=YES, 2=NO";z
190 if z=1 goto 50
200 data 65,23,17,56,39