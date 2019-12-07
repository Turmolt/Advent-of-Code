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
  (do
    (println (str "C: " c " M: " m " P: " p ))
    (loop [i m n (first p) r (rest p)]
      (if-not (nil? n)
        (let [s (solve-phase c i n)]
          (do
            (println s)
            (recur s (first r) (rest r))))
        [i p]))))

(defn check-phase-recursively [c m p]
  (do
    (println (str "C: " c " M: " m " P: " p))
    (loop [i m n (first p) r (rest p) t c]
      (do (println (str "I: " i))
          (if-not (nil? n)
            (do
              (println (str c "\n" i "\n" n))
              (let [s (solve-phase c i n)]
                (do (println s)
                    (recur s (first r) (rest r) t))))
            (let [s (cpu/solve t i 0)]
              (do
                (println (str "S: " s))
                (recur s nil nil (second s)))))))))

(defn part-one [] (apply max (map (comp first #(check-phase (u/input-csv 7) 0 %)) (vec (map vec (permutations [0 1 2 3 4]))))))

(time (part-one))
;; => 117312
;; => "Elapsed time: 114.26 msecs"

(println (u/input-csv 0))

(cpu/solve (u/input-csv 7) 0 0)

(first (check-phase (u/input-csv 0) 0 [9 7 8 5 6]))