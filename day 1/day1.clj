(ns adventofcode.day1)
(use 'clojure.java.io)

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

(defn read_lines [f]
  (with-open [r (reader f)]
    (doall (line-seq r))))

(defn solve [l] (println (reduce calculate2 0 l)))

(solve (map read-string (read_lines "day 1/input.txt")))

