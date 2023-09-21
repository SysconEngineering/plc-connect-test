# PLC-Connect-Test

## 1. 사용 용도

---

- PLC 관련하여 사전 고객사 시나리오 테스트 (팀 내부 테스트 용도)

## 2.  Gathering-PLC vs PLC Test Tool

---

### 🟢 Gathering PLC

- 현장 셋업 / 모니터링 용
- DB O
- 관제 연동 O

### 🟠 PLC Test Tool

- 사전 고객사 시나리오 테스트
- DB X → JSON 파일 읽어서 사용
- 관제 연동 없이 독립적인 테스트 툴

## 3. 작업 내용

---

1. Modbus 연결 (완료)
2. Modbus 실시간 메모리 번지 데이터 Read (완료)
3. MC(Mitsubishi 자체 프로토콜) 연결  (완료)
4. MC 실시간 메모리 번지 데이터 Read (완료)
5. Device 정보 ( 설비 기본정보 및 Device Data 관련) Json 파일 읽어서 셋팅  (완료)
6. Mitsubishi 실시간 데이터 Read 테스트 완료 (완료)
7. Modbus / Mitsubishi 데이터 Write 구현(+ 테스트 완료) (완료)
8. 모니터링 화면에 실시간 데이터 표시 (작업중 60% / 보류)

## 4. To Do

- 약속한 시나리오 대로 자동 Read / Write 되도록 구현

## 참고

---

- PLC 별 사용하는 프로토콜 정리

| plc/protocol | Modbus | OPC-UA | S7(지멘스 자체 Protocol) | MC(미츠비시 자체 Protocol) |
| --- | --- | --- | --- | --- |
| Simense  | O | O | O |  |
| LS | O | O |  |  |
| Mitsubishi | O | O |  | O |
