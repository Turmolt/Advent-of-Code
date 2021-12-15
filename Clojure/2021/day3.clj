(ns adventofcode.2021.day3
  (:require [adventofcode.util :as u]))

(def data (u/input-lsv 2021 3))

(defn convert-to-array [input] (mapv (comp #(Integer/parseInt %) str) input))

(def converted-data (mapv convert-to-array data))

(def bool-to-int {true 1 false 0})

(defn tally-target [target] (mapv (fn [input] [(bool-to-int (= 0 input)) (bool-to-int (= 1 input))]) target))

(def tallied-data (mapv tally-target converted-data))

(defn add-tallies [t1 t2]
  (map-indexed
   (fn [idx tally]
     (let [target (nth t2 idx)]
       [(+ (first tally) (first target))
        (+ (second tally) (second target))])) t1))

(def reduced-data (reduce add-tallies tallied-data))

(def reduced-converted-data (mapv (fn [[zero one]] (if (> zero one) 0 1)) reduced-data))


(def gamma (Integer/parseInt (reduce str (mapv (fn [[zero one]] (if (> zero one) 0 1)) reduced-data)) 2))

(def epsilon (Integer/parseInt (reduce str (mapv (fn [[zero one]] (if (< zero one) 0 1)) reduced-data)) 2))

(def answer-part1 (* gamma epsilon))

(defn solve-p2 [seeking]
  (loop [idx 0 current (first tallied-data) remaining (rest tallied-data) survivors []]
    (let [target (nth reduced-converted-data idx)
          defendent (nth current idx)
          comparison (nth defendent target)]
      (if (> 0 comparison)
        (let [new-survivors (conj survivors current)]
          (if (not-empty remaining)
            (recur idx (first remaining) (rest remaining) new-survivors)))
        (if (not-empty remaining)
          (recur idx (first remaining) (rest remaining) survivors))))))

; i did part 2 in C# because.. i am a C# programmer

;010100110101