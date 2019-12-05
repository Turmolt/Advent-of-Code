(ns adventofcode.day5
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(defn opcode [n]
  (#(concat (repeat (- 5 (count %)) 0) %) (->> n str (map (comp read-string str)))))

(def steps
  {1 4
   2 4
   3 2
   4 2})

(defn value [c o n d]
  (case d
    true (case o
           0 (nth c n)
           1 n)
    false n)
  )

(defn execute [s c m]
  (let [o (opcode (first s))
        d (steps (last o))
        n1 (value c (nth o 2) (nth s 1) (= d 4))
        n2 (value c (nth o 1) (nth s 2) (= d 4))
        n3 (nth s 3)]
    (case (last o)
      1 (assoc c n3 (+ n1 n2))
      2 (assoc c n3 (* n1 n2))
      3 (assoc c n1 m)
      4 (do
          (println (str "Output: " (nth c n1)))
          c))))

(defn solve [c m]
  (loop [i 0 n c]
    (if (= 99 (nth n i))
      (println (str "Halt!"))
      (if (>= (+ 4 i) (count n)) (println n)
          (let [s (subvec n i (+ 4 i))]
            (recur (+ i (steps (last (opcode (first s))))) (execute s n m)))))))

(time (solve (u/input-csv 5) 1))