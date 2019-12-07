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

(defn solve-phase-interuptable [c m p]
  (let [op (cpu/opcode (nth c 0))
        step (cpu/steps (last op))
        s (subvec c 0 step)]
        (->> (cpu/execute s c p 0)
             (second)
             (#(cpu/solve-interuptable % m step)))))

(defn check-phase [c m p]
    (loop [i m n (first p) r (rest p)]
      (if-not (nil? n)
        (let [s (solve-phase c i n)]
            (recur s (first r) (rest r)))
        [i p])))

(defn check-phase-recursively [c m p]
    (loop [i m n (first p) r (rest p) mem (vec (take 5 (repeat [0 c]))) memind 0 coll []]
          (if (nil? i)
            coll
          (if-not (nil? n)
              (let [s (solve-phase-interuptable c i n)]
                (recur (first s) (first r) (rest r) (assoc mem memind s) (mod (inc memind) 5) (if (= 4 memind) (conj coll (first s)) coll)))
              (let [s (cpu/solve-interuptable (second (nth mem memind)) i (last (nth mem memind)))]
                  (if (nil? (first s))
                    coll
                    (recur (first s) nil nil (assoc mem memind s) (mod (inc memind) 5) (if (= 4 memind) (conj coll (first s)) coll))))))))

(defn part-one [] (apply max (map (comp first #(check-phase (u/input-csv 7) 0 %)) (vec (map vec (permutations [0 1 2 3 4]))))))

(defn part-two [] (apply max (map (partial last) (map #(check-phase-recursively (u/input-csv 7) 0 %) (vec (map vec (permutations [9 8 7 6 5])))))))

(time (part-one))
;; => 117312
;; => "Elapsed time: 103.24639 msecs"

(time (part-two))
;; => 1336480
;; => "Elapsed time: 222.768927 msecs"