(ns adventofcode.day3
  (:require [clojure.string :as str]))

(defn abs [n] (max n (- n)))

(def input
  (map #(str/split % #",") (str/split (slurp "day 3/input.txt") #"\n")))

(defn movement [d]
  (case d
    "U" [0 1]
    "D" [0 -1]
    "R" [1 0]
    "L" [-1 0]))

(defn between [i s f]
  (and (<= s i) (>= f i)))

(defn create-line [ins]
  (loop [s [0 0]
         i 0]
    (if (<= (count ins) i)
      s
      (let [t (nth ins i)
            n (Integer. (re-find #"\d+" t))
            dir (str (first t))]
        (recur (concat s (repeat n (movement dir))) (inc i))))))

(defn create-and-intersect [i]
  (let [l1 (create-line (first i))
        l2 (create-line (second i))]
    (println (apply clojure.set/intersection (set l1) (set l2)))))

(create-and-intersect input)