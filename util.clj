(ns adventofcode.util
  (:require [clojure.java.io :as io]
            [clojure.string :as str]))

(defn path [d]
  (str "input/day" d ".txt"))

(defn input [d] (slurp (path d)))

;TODO: merge two methods below
(defn input-lcsv [d]
  (map #(str/split % #",") (str/split-lines (input d))))

(defn input-csv [d]
  (vec (map read-string (str/split (input d) #","))))

(defn input-lsv [d]
  (with-open [r (io/reader (path d))]
    (doall (line-seq r))))

(defn find-first [pred coll] (first (filter pred coll)))

(defn gcd [x y] (if (zero? y) x (recur y (mod x y))))

(defn lcm [x y] (/ (* x y) (gcd x y)))