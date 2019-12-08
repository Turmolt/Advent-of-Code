(ns adventofcode.day7
  (:require [adventofcode.util :as u]
            [adventofcode.day5 :as cpu]))

(defn permutations [s]
  (lazy-seq 
   (if (seq (rest s))
     (apply concat (for [x s]
                     (map #(cons x %) (permutations (remove #{x} s)))))
     [s])))

(defn solve-phase [c m p cmd]
  (let [op (cpu/opcode (nth c 0))
        step (cpu/steps (last op))
        s (subvec c 0 step)]
    (->> (cpu/execute s c p 0)
         (second)
         (#(cmd % m step)))))

(defn check-phase [c m p]
    (loop [i m n (first p) r (rest p)]
      (if-not (nil? n)
        (let [s (solve-phase c i n cpu/solve)]
            (recur s (first r) (rest r)))
        [i p])))

(defn check-phase-recursively [c m p]
    (loop [i m n (first p) r (rest p)
           mem (vec (repeat 5 [0 c])) mi 0 coll []]
          (if (nil? i)
            coll
          (if-not (nil? n)
              (let [s (solve-phase c i n cpu/solve-interuptable)]
                (recur (first s) (first r) (rest r) (assoc mem mi s)
                       (mod (inc mi) 5) (if (= 4 mi) (conj coll (first s)) coll)))
              (let [s (cpu/solve-interuptable (second (nth mem mi)) i (last (nth mem mi)))]
                  (if (nil? (first s))
                    coll
                    (recur (first s) nil nil (assoc mem mi s)
                           (mod (inc mi) 5) (if (= 4 mi) (conj coll (first s)) coll))))))))

(defn part-one [] (apply max (map (comp first #(check-phase (u/input-csv 7) 0 %)) (vec (map vec (permutations [0 1 2 3 4]))))))

(defn part-two [] (apply max (map (partial last) (map #(check-phase-recursively (u/input-csv 7) 0 %) (vec (map vec (permutations [9 8 7 6 5])))))))

(time (part-one))
;; => 117312
;; => "Elapsed time: 97.507849 msecs"

(time (part-two))
;; => 1336480
;; => "Elapsed time: 206.364227 msecs"