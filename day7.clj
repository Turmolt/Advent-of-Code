(ns adventofcode.day7
  (:require [adventofcode.util :as u]
            [adventofcode.day5 :as cpu]
            [clojure.string :as str]
            [clojure.set :as set]))

(defn permutations [s]
  (lazy-seq 
   (if (seq (rest s))
     (apply concat (for [x s]
                     (map #(cons x %) (permutations (remove #{x} s)))))
     [s])))

(defn solve-phase [c m p]
  (let [op (cpu/opcode (nth c 0))
        step (cpu/steps (last op))
        s (subvec c 0 step)]
    (->> (cpu/execute s c p 0)
         (second)
         (#(cpu/solve % m step)))))

(defn check-phase [c m p]
  (loop [i m n (first p) r (rest p)]
    (if-not (nil? n)
      (let [s (solve-phase c i n)]
        (recur s (first r) (rest r)))
      [i p])))

(defn part-one [] (apply max (flatten (map #(check-phase (u/input-csv 7) 0 %) (vec (map vec (permutations [0 1 2 3 4])))))))

(time (part-one))
;; => 117312
;; => "Elapsed time: 114.26 msecs"