(ns adventofcode.day13
  (:require [adventofcode.util :as u]
            [adventofcode.day5 :as cpu]))

(def blank-memory-a [(cpu/create-memory (u/input-csv 13)) 0 0])

(def blank-memory-b [(cpu/create-memory (u/input-csv "13b")) 0 0])

(def tile {0 \e 1 \w 2 \b 3 \p 4 \o})

(defn run-cpu [mem input idx ridx len]
  (loop [cpu-output (cpu/solve-interuptable mem input idx ridx) n 1 output {:memory [] :output []}]
    (if (or (= len n) (nil? (first cpu-output)))
      (assoc (assoc output :memory (rest cpu-output)) :output (conj (output :output) (first cpu-output)))
      (recur (cpu/solve-interuptable (second cpu-output) input (nth cpu-output 2) (last cpu-output))
             (inc n) (assoc output :output (conj (output :output) (first cpu-output)))))))

(defn extract-board [memory]
  (loop [cpu-output (run-cpu (first memory) 0 (second memory) (last memory) 3) output {}]
    (if (some nil? (cpu-output :output))
      output
      (recur (run-cpu (first (cpu-output :memory)) 0 (second (cpu-output :memory)) (last (cpu-output :memory)) 3)
             (conj output [[(first (cpu-output :output)) (second (cpu-output :output))] (last (cpu-output :output))])))))

(defn part-one []
  (->> blank-memory-a
       (extract-board)
       (map val)
       (remove #(not (= 2 %)))
       (count)))

(part-one)