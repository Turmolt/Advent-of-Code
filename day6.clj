(ns adventofcode.day6
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def input 
  (->> (u/input-lsv 6)
       (map #(str/split % #"\)"))
       (map (comp vec reverse))
       (into {})))

(defn build-chain [d k]
  (take-while identity (rest (iterate d k))))

(defn part-one []
  (->> (keys input)
       (map (comp count (partial #(build-chain input %))))
       (reduce +)))

(defn part-two []  
  (let [y (build-chain input "YOU")
        s (build-chain input "SAN")
        i (clojure.set/intersection (set y) (set s))]
    (->> i
         (map #(+ (.indexOf y %) (.indexOf s %)))
         (apply min))))

(time (part-one))
;; => 200001
;; => "Elapsed time: 64.2113 msecs"

(time (part-two))
;; => 379
;; => "Elapsed time: 3.5618 msecs"