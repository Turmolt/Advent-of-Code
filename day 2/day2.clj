(ns adventofcode.day2)
(require '[clojure.java.io :as io]
         '[clojure.string :as str])

(defn catch-fire [c i o] (assoc (assoc c 1 i) 2 o))

(defn read-split [s]
  (vec (map read-string (str/split (slurp s) #","))))

(defn execute [s c]
  (let [n1 (nth c (nth s 1))
        n2 (nth c (nth s 2))
        n3 (nth s 3)]
    (case (first s)
      1 (assoc c n3 (+ n1 n2))
      2 (assoc c n3 (* n1 n2)))))

(defn solve [c]
  (loop [i 0 n c]
    (if (>= (* i 4) (count n)) n
      (let [s (subvec n (* i 4) (+ 4 (* i 4)))]
        (if (= 99 (first s))
          n
          (recur (inc i) (execute s n)))))))

(defn find-pairs [p]
  (let [c (read-split p)]
    (loop [n 0 v 0 i 0]
      (if (= n 100)
        (println "Exceeded!")
        (let [a (solve (catch-fire c n v))]
          (if (= (first a) 19690720)
            (println (str "n: " n " v: " v " r: " (first a)))
            (recur (int (/ i 99)) (mod i 100) (inc i))))))))

(find-pairs "day 2/input.txt")