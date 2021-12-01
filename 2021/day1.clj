(ns adventofcode.2021.day1
  (:require [adventofcode.util :as u]))

; part 1
(def data (map #(Integer/parseInt %) (u/input-lsv 2021 1)))

(def offset-data (rest data))

(defn count-increases [col] (count (filter (fn [[v1 v2]] (< v1 v2)) col)))

(count-increases (partition 2 (interleave data offset-data)))

;part 2
(def double-offset-data (rest offset-data))

(def compiled-data (partition 3 (interleave data offset-data double-offset-data)))

(def summed-data (map (fn [col] (reduce + col)) compiled-data))

(def offset-summed-data (rest summed-data))

(count-increases (partition 2 (interleave summed-data offset-summed-data)))