#!/bin/bash
for dir in $(ls -R | grep -e 'bin:' ); do
   path=$(echo -n $dir | sed -e 's/://')
   rm -rvf $path
done
for dir in $(ls -R | grep -e 'obj:' ); do
   path=$(echo -n $dir | sed -e 's/://')
   rm -rvf $path
done
