(ns adventofcode.day13
  (:require [adventofcode.util :as u]
            [adventofcode.day5 :as cpu]))

(def blank-memory-a [(cpu/create-memory (u/input-csv 13)) 0 0])

(def blank-memory-b (cpu/create-memory (u/input-csv "13b")))

(def tile {0 " " 1 "X" 2 "#" 3 "=" 4 "o"})

(defn display-board [b]
  (let [coords (remove #(= [-1 0] %) (remove nil? (keys b)))
        minX (first (first (sort-by first coords)))
        maxX (first (last (sort-by first coords)))
        minY (last (first (sort-by second coords)))
        maxY (last (last (sort-by second coords)))]
    (for [y (range minY (inc maxY))
          x (range minX (inc maxX))]
      (if (< x maxX) (print (tile (b [x y])))
          (print (tile (b [x y])) "\n")))))

(defn run-cpu [mem input idx ridx len]
  (loop [cpu-output (cpu/solve-interuptable mem input idx ridx) n 1 output {:memory [] :output []}]
    (if (or (= len n) (nil? (first cpu-output)))
      (assoc (assoc output :memory (rest cpu-output)) :output (conj (output :output) (first cpu-output)))
      (recur (cpu/solve-interuptable (second cpu-output) input (nth cpu-output 2) (last cpu-output))
             (inc n) (assoc output :output (conj (output :output) (first cpu-output)))))))

(defn run-to-end [memory]
  (loop [cpu-output (run-cpu (first memory) 0 (second memory) (last memory) 3) output {}]
    (if (some nil? (cpu-output :output))
      output
      (recur (run-cpu (first (cpu-output :memory)) 0 (second (cpu-output :memory)) (last (cpu-output :memory)) 3)
             (assoc output [(first (cpu-output :output)) (second (cpu-output :output))] (last (cpu-output :output)))))))

(def in (atom 0))

(def should-ask (atom true))

(defn run-cpu-with-input [mem idx ridx]0
  (loop [cpu-output (cpu/solve-input mem @in idx ridx true)
         output {:memory [] :board {}}
         buffer []]
    (if (nil? (first cpu-output))
      (assoc (assoc output :memory (rest cpu-output)) :board (assoc (output :board) [(first buffer) (second buffer)] (last buffer)))
      (do
        (if (= \I (first cpu-output))
          (do (doall (display-board (assoc (output :board) [(first buffer) (second buffer)] (last buffer))))
              (println "Score: " ((output :board) [-1 0]))
              (swap! in (fn [x] (let [p (first (filter #(= (val %) 3) (output :board)))
                                      b (first (filter #(= (val %) 4) (assoc (output :board) [(first buffer) (second buffer)] (last buffer))))]
                                  (- (first (key b)) (first (key p))))))
              (swap! should-ask (fn [x] (identity false))))
          (swap! should-ask (fn [x] (identity true))))
        (let [op (cpu/solve-input (second cpu-output) @in (nth cpu-output 2) (last cpu-output) @should-ask)]
          (if (= 3 (count buffer))
            (recur op
                   (assoc output :board (assoc (output :board) [(first buffer) (second buffer)] (last buffer)))
                   (if (number? (first cpu-output))
                     [(first cpu-output)]
                     []))
            (recur (cpu/solve-input (second cpu-output) @in (nth cpu-output 2) (last cpu-output) @should-ask)
                   output
                   (if (number? (first cpu-output))
                     (conj buffer (first cpu-output))))))))))

(defn part-one []
  (->> blank-memory-a
       (run-to-end)
       (map val)
       (remove #(not (= 2 %)))
       (count)))

(defn part-two []
  (let [output (run-cpu-with-input blank-memory-b 0 0)
        board (output :board)]
    (doall (display-board board))
    (println "Final Score: " (board [-1 0]))))

;(part-one)

;(part-two)