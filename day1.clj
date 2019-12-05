(ns adventofcode.day1
  (:require [adventofcode.util :as u]))

(defn calculate [i]
  (- (int (Math/floor (/ i 3))) 2))

(defn calculate_additive [i]
  (loop [f1 (calculate i)
         f2 0]
    (if (>= 0 f1)
      f2
      (recur (calculate f1) (+ f2 f1)))))

(defn calculate2 [o i]
  (+ o (calculate_additive i)))

(defn solve [l] (println (reduce calculate2 0 l)))

(solve (map read-string (u/input-lsv 1)))


(println (u/input-lsv 1))
