(ns adventofcode.day6
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def input (into {} (map (comp vec reverse) (map #(str/split % #"\)") (u/input-lsv 6)))))

(defn build-chain [d k i]
  (loop [o (if i 0 []) key k]
    (if (contains? d key)
      (recur (if i (inc o) (conj o (d key))) (d key))
      o)))

(defn part-one []
  (->> (keys input)
       (map (partial #(build-chain input % true)))
       (reduce +)))

(defn part-two []  
  (let [y (build-chain input "YOU" false)
        s (build-chain input "SAN" false)
        i (clojure.set/intersection (set y) (set s))]
    (->> i
         (map #(+ (.indexOf y %) (.indexOf s %)))
         (apply min))
    ))

(time (part-one))
;; => 200001
;; => "Elapsed time: 64.2113 msecs"

(time (part-two))
;; => 379
;; => "Elapsed time: 3.5618 msecs"
