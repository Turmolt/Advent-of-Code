(ns adventofcode.day6
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def input (into {} (map (comp vec reverse) (map #(str/split % #"\)") (u/input-lsv 6)))))

(defn build-chain [d k i]
  (loop [o (if (= i \I) 0 []) key k]
    (if (contains? d key)
      (recur (if (= i \I) (inc o) (conj o (d key))) (d key))
      o)))

(defn part-one []
  (reduce + (map (partial #(build-chain input % \I)) (keys input))))

(defn part-two []  (let [y (build-chain input "YOU" \C)
                         s (build-chain input "SAN" \C)
                         i (clojure.set/intersection (set y) (set s))]
                     (apply min (map #(+ (.indexOf y %) (.indexOf s %)) i))))

(time (part-one))
;; => 200001
;; => "Elapsed time: 61.6544 msecs"

(time (part-two))
;; => 379
;; => "Elapsed time: 5.5923 msecs"