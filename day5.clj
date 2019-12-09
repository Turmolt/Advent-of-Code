(ns adventofcode.day5
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(defn opcode [n]
  (#(concat (repeat (- 5 (count %)) 0) %) (->> n str (map (comp read-string str)))))

(def steps
  {1 4, 2 4, 3 2, 4 2, 5 3, 6 3, 7 4, 8 4, 9 2})

(defn fetch [memory n]
  (if-not (contains? memory n) 0 (memory n)))

(defn put [memory n value]
  (assoc memory n value))

(defn create-memory [memory]
  (->> memory
       (map-indexed (fn [idx itm] [idx itm]))
       (into {})))

(defn value [c o n d ri]
  (case o
    0 (if d (fetch c n) n)
    1 n
    2 (if d (fetch c (+ n ri)) (+ n ri))))

(defn get-code [memory start step]
  (->> (vec (take step (iterate inc start)))
       (map #(fetch memory %))))

(defn execute [s c m i ri]
  (let [o (opcode (first s))
        d (steps (last o))
        ni (+ i d)
        n1m (nth o 2) n2m (nth o 1) n3m (nth o 0)
        n1 (value c n1m (nth s 1) (>= d 3)  ri)
        n2 (if (<= 3 (count s)) (value c n2m (nth s 2) (>= d 3) ri) nil)
        n3 (if (<= 4 (count s)) (value c n3m (nth s 3) (some (partial = (last o)) [5 6])  ri) nil)]
    (case (last o)
      1 [ni (put c n3 (+ n1 n2)) ri]
      2  [ni (put c n3 (* n1 n2)) ri]
      3 (case n1m
          2 [ni (put c (+ ri (second s)) m) ri]
          [ni (put c (second s) m) ri])
      4 [ni c (fetch c n1) ri]
      5 (if-not (zero? n1) [n2 c ri] [ni c ri])
      6 (if (zero? n1) [n2 c ri] [ni c ri])
      7 (case n3m
          2 [ni (put c (fetch c n3) (if (< n1 n2) 1 0)) ri]
          [ni (put c n3 (if (< n1 n2) 1 0)) ri])
      8 (case n3m
          2 [ni (put c (fetch c n3) (if (= n1 n2) 1 0)) ri]
          [ni (put c n3 (if (= n1 n2) 1 0)) ri])
      9 (case n1m
          0 [ni c (+ ri (fetch c (second s)))]
          2 [ni c (+ ri (fetch c (+ ri (second s))))]
          [ni c (+ ri (second s))]))))

(defn solve [c m si]
  (loop [i si n c r 0 ri 0]
    (if (= 99 (fetch n i))
      r
      (let [op (opcode (fetch n i))
            step (steps (last op))]
        (let [s (vec (get-code n i step))
              o (execute s n m i ri)]
          (recur (first o) (second o) (first (take-last 2 o)) (peek o)))))))

(defn solve-interuptable [c m si]
  (loop [i si n c r nil ri 0]
      (if (number? r)
        [r n i]
        (if (= 99 (nth n i))
          nil
          (let [op (opcode (fetch n i))
                step (steps (last op))]
            (if (>= (+ step i) (count n)) (println n)
                (let [s (get-code n i step)
                      o (execute s n m i ri)]
                  (recur (first o) (second o) (first (take-last 2 o)) (peek o)))))))))

(time (solve (create-memory (u/input-csv 5)) 5 0))