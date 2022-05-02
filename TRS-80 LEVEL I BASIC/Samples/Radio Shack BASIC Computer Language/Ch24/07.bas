10 cls
20 y=1:n=0
30 p. "Answer these questions with 'YES' or 'NO'.":P.
40 in. "Has the cat been put out";a
50 in. "Is the porch light turned off";b
60 in. "Are all doors and windows locked";c
70 in. "Is the television turned off";d
80 in. "Did you turn the thermostat down";e
90 p.:p.
100 if (a=n)+(b=n)+(c=n)+(d=n)+(e=n) then 130
120 p."             GOODNIGHT":end
130 p."Something has not been done. Do not go to bed"
140 p."until you find the problem!"
150 goto 40