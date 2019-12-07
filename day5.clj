(ns adventofcode.day5
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(defn opcode [n]
  (#(concat (repeat (- 5 (count %)) 0) %) (->> n str (map (comp read-string str)))))

(def steps
  {1 4
   2 4
   3 2
   4 2
   5 3
   6 3
   7 4
   8 4})

(defn value [c o n d]
  (if d (case o
          0  (nth c n)
          1 n
          (do (println "Break!")
              -0.1))
      n))

(defn execute [s c m i]
  (let [o (opcode (first s))
        d (steps (last o))
        ni (+ i d)
        n1 (value c (nth o 2) (nth s 1) (>= d 3))
        n2 (if (<= 3 (count s)) (value c (nth o 1) (nth s 2) (>= d 3)) nil)
        n3 (if (<= 4 (count s)) (value c (nth o 0) (nth s 3) (some (partial = (last o)) [5 6])) nil)]
    (case (last o)
      1 [ni (assoc c n3 (+ n1 n2))]
      2 [ni (assoc c n3 (* n1 n2))]
      3 [ni (assoc c n1 m)]
      4 (do
          (println (str "Output: " (nth c n1)))
          [ni c (nth c n1)])
      5 (if-not (zero? n1) [n2 c] [ni c])
      6 (if (zero? n1) [n2 c] [ni c])
      7 [ni (assoc c n3 (if (< n1 n2) 1 0))]
      8 [ni (assoc c n3 (if (= n1 n2) 1 0))]
      (do (println "Break!!")
          [0 [99]]))))

(defn solve [c m si]
  (loop [i si n c r 0]
    (if (= 99 (nth n i))
      (do (println (str "Halt!"))
          r)
      (let [op (opcode (nth n i))
            step (steps (last op))]
        (if (>= (+ step i) (count n)) (println n)
            (let [s (subvec n i (+ step i))
                  o (execute s n m i)]
              (recur (first o) (second o) (peek o))))))))

(defn solve-interuptable [c m si]
  (loop [i si n c r nil]
      (if (number? r)
        [r n i]
        (if (= 99 (nth n i))
          (do (println (str "Halt!!"))
              nil)
          (let [op (opcode (nth n i))
                step (steps (last op))]
            (if (>= (+ step i) (count n)) (println n)
                (let [s (subvec n i (+ step i))
                      o (execute s n m i)]
                  (recur (first o) (second o) (peek o)))))))))


(time (solve (u/input-csv 5) 5 0))