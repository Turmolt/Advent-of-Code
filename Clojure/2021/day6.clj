(ns adventofcode.2021.day6
  (:require [adventofcode.util :as u]))

(reduce (fn [v n] (update v n inc)) (vec (repeat 9 (long 0))) [6,0,6,4,5,6,0,1,1,2,6,0,1,1,1,2,2,3,3,4,6,7,8,8,8,8])
(def data (reduce (fn [v n] (update v n inc)) (vec (repeat 9 (long 0))) (u/input-csv-long 2021 6)))

(defn step [fish-map]
  (let [zero (first fish-map)]
    (-> fish-map
        rest
        vec
        (conj zero)
        (update 6 #(+ % zero)))))

(apply + (last (take 257 (iterate step data))))