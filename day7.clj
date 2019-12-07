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
    (if-not (empty? r)
      (let [s (solve-phase c i n)]
        (recur s (first r) (rest r)))
      [i p])))

(apply max-key (map (fn [[x y]] ([(int x) y])) (map #(check-phase (u/input-csv 7) 0 %) (vec (map vec (permutations [0 1 2 3 4]))))))