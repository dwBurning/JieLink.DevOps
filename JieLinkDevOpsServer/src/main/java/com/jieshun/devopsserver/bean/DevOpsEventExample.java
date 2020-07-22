package com.jieshun.devopsserver.bean;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class DevOpsEventExample {
    protected String orderByClause;

    protected boolean distinct;

    protected List<Criteria> oredCriteria;

    public DevOpsEventExample() {
        oredCriteria = new ArrayList<Criteria>();
    }

    public void setOrderByClause(String orderByClause) {
        this.orderByClause = orderByClause;
    }

    public String getOrderByClause() {
        return orderByClause;
    }

    public void setDistinct(boolean distinct) {
        this.distinct = distinct;
    }

    public boolean isDistinct() {
        return distinct;
    }

    public List<Criteria> getOredCriteria() {
        return oredCriteria;
    }

    public void or(Criteria criteria) {
        oredCriteria.add(criteria);
    }

    public Criteria or() {
        Criteria criteria = createCriteriaInternal();
        oredCriteria.add(criteria);
        return criteria;
    }

    public Criteria createCriteria() {
        Criteria criteria = createCriteriaInternal();
        if (oredCriteria.size() == 0) {
            oredCriteria.add(criteria);
        }
        return criteria;
    }

    protected Criteria createCriteriaInternal() {
        Criteria criteria = new Criteria();
        return criteria;
    }

    public void clear() {
        oredCriteria.clear();
        orderByClause = null;
        distinct = false;
    }

    protected abstract static class GeneratedCriteria {
        protected List<Criterion> criteria;

        protected GeneratedCriteria() {
            super();
            criteria = new ArrayList<Criterion>();
        }

        public boolean isValid() {
            return criteria.size() > 0;
        }

        public List<Criterion> getAllCriteria() {
            return criteria;
        }

        public List<Criterion> getCriteria() {
            return criteria;
        }

        protected void addCriterion(String condition) {
            if (condition == null) {
                throw new RuntimeException("Value for condition cannot be null");
            }
            criteria.add(new Criterion(condition));
        }

        protected void addCriterion(String condition, Object value, String property) {
            if (value == null) {
                throw new RuntimeException("Value for " + property + " cannot be null");
            }
            criteria.add(new Criterion(condition, value));
        }

        protected void addCriterion(String condition, Object value1, Object value2, String property) {
            if (value1 == null || value2 == null) {
                throw new RuntimeException("Between values for " + property + " cannot be null");
            }
            criteria.add(new Criterion(condition, value1, value2));
        }

        public Criteria andIdIsNull() {
            addCriterion("id is null");
            return (Criteria) this;
        }

        public Criteria andIdIsNotNull() {
            addCriterion("id is not null");
            return (Criteria) this;
        }

        public Criteria andIdEqualTo(Integer value) {
            addCriterion("id =", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotEqualTo(Integer value) {
            addCriterion("id <>", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdGreaterThan(Integer value) {
            addCriterion("id >", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdGreaterThanOrEqualTo(Integer value) {
            addCriterion("id >=", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdLessThan(Integer value) {
            addCriterion("id <", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdLessThanOrEqualTo(Integer value) {
            addCriterion("id <=", value, "id");
            return (Criteria) this;
        }

        public Criteria andIdIn(List<Integer> values) {
            addCriterion("id in", values, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotIn(List<Integer> values) {
            addCriterion("id not in", values, "id");
            return (Criteria) this;
        }

        public Criteria andIdBetween(Integer value1, Integer value2) {
            addCriterion("id between", value1, value2, "id");
            return (Criteria) this;
        }

        public Criteria andIdNotBetween(Integer value1, Integer value2) {
            addCriterion("id not between", value1, value2, "id");
            return (Criteria) this;
        }

        public Criteria andEventTypeIsNull() {
            addCriterion("event_type is null");
            return (Criteria) this;
        }

        public Criteria andEventTypeIsNotNull() {
            addCriterion("event_type is not null");
            return (Criteria) this;
        }

        public Criteria andEventTypeEqualTo(Integer value) {
            addCriterion("event_type =", value, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeNotEqualTo(Integer value) {
            addCriterion("event_type <>", value, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeGreaterThan(Integer value) {
            addCriterion("event_type >", value, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeGreaterThanOrEqualTo(Integer value) {
            addCriterion("event_type >=", value, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeLessThan(Integer value) {
            addCriterion("event_type <", value, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeLessThanOrEqualTo(Integer value) {
            addCriterion("event_type <=", value, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeIn(List<Integer> values) {
            addCriterion("event_type in", values, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeNotIn(List<Integer> values) {
            addCriterion("event_type not in", values, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeBetween(Integer value1, Integer value2) {
            addCriterion("event_type between", value1, value2, "eventType");
            return (Criteria) this;
        }

        public Criteria andEventTypeNotBetween(Integer value1, Integer value2) {
            addCriterion("event_type not between", value1, value2, "eventType");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountIsNull() {
            addCriterion("remote_account is null");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountIsNotNull() {
            addCriterion("remote_account is not null");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountEqualTo(String value) {
            addCriterion("remote_account =", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountNotEqualTo(String value) {
            addCriterion("remote_account <>", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountGreaterThan(String value) {
            addCriterion("remote_account >", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountGreaterThanOrEqualTo(String value) {
            addCriterion("remote_account >=", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountLessThan(String value) {
            addCriterion("remote_account <", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountLessThanOrEqualTo(String value) {
            addCriterion("remote_account <=", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountLike(String value) {
            addCriterion("remote_account like", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountNotLike(String value) {
            addCriterion("remote_account not like", value, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountIn(List<String> values) {
            addCriterion("remote_account in", values, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountNotIn(List<String> values) {
            addCriterion("remote_account not in", values, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountBetween(String value1, String value2) {
            addCriterion("remote_account between", value1, value2, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andRemoteAccountNotBetween(String value1, String value2) {
            addCriterion("remote_account not between", value1, value2, "remoteAccount");
            return (Criteria) this;
        }

        public Criteria andContactNameIsNull() {
            addCriterion("contact_name is null");
            return (Criteria) this;
        }

        public Criteria andContactNameIsNotNull() {
            addCriterion("contact_name is not null");
            return (Criteria) this;
        }

        public Criteria andContactNameEqualTo(String value) {
            addCriterion("contact_name =", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameNotEqualTo(String value) {
            addCriterion("contact_name <>", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameGreaterThan(String value) {
            addCriterion("contact_name >", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameGreaterThanOrEqualTo(String value) {
            addCriterion("contact_name >=", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameLessThan(String value) {
            addCriterion("contact_name <", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameLessThanOrEqualTo(String value) {
            addCriterion("contact_name <=", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameLike(String value) {
            addCriterion("contact_name like", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameNotLike(String value) {
            addCriterion("contact_name not like", value, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameIn(List<String> values) {
            addCriterion("contact_name in", values, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameNotIn(List<String> values) {
            addCriterion("contact_name not in", values, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameBetween(String value1, String value2) {
            addCriterion("contact_name between", value1, value2, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactNameNotBetween(String value1, String value2) {
            addCriterion("contact_name not between", value1, value2, "contactName");
            return (Criteria) this;
        }

        public Criteria andContactPhoneIsNull() {
            addCriterion("contact_phone is null");
            return (Criteria) this;
        }

        public Criteria andContactPhoneIsNotNull() {
            addCriterion("contact_phone is not null");
            return (Criteria) this;
        }

        public Criteria andContactPhoneEqualTo(String value) {
            addCriterion("contact_phone =", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneNotEqualTo(String value) {
            addCriterion("contact_phone <>", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneGreaterThan(String value) {
            addCriterion("contact_phone >", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneGreaterThanOrEqualTo(String value) {
            addCriterion("contact_phone >=", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneLessThan(String value) {
            addCriterion("contact_phone <", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneLessThanOrEqualTo(String value) {
            addCriterion("contact_phone <=", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneLike(String value) {
            addCriterion("contact_phone like", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneNotLike(String value) {
            addCriterion("contact_phone not like", value, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneIn(List<String> values) {
            addCriterion("contact_phone in", values, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneNotIn(List<String> values) {
            addCriterion("contact_phone not in", values, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneBetween(String value1, String value2) {
            addCriterion("contact_phone between", value1, value2, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andContactPhoneNotBetween(String value1, String value2) {
            addCriterion("contact_phone not between", value1, value2, "contactPhone");
            return (Criteria) this;
        }

        public Criteria andOperatorDateIsNull() {
            addCriterion("operator_date is null");
            return (Criteria) this;
        }

        public Criteria andOperatorDateIsNotNull() {
            addCriterion("operator_date is not null");
            return (Criteria) this;
        }

        public Criteria andOperatorDateEqualTo(Date value) {
            addCriterion("operator_date =", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateNotEqualTo(Date value) {
            addCriterion("operator_date <>", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateGreaterThan(Date value) {
            addCriterion("operator_date >", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateGreaterThanOrEqualTo(Date value) {
            addCriterion("operator_date >=", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateLessThan(Date value) {
            addCriterion("operator_date <", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateLessThanOrEqualTo(Date value) {
            addCriterion("operator_date <=", value, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateIn(List<Date> values) {
            addCriterion("operator_date in", values, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateNotIn(List<Date> values) {
            addCriterion("operator_date not in", values, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateBetween(Date value1, Date value2) {
            addCriterion("operator_date between", value1, value2, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andOperatorDateNotBetween(Date value1, Date value2) {
            addCriterion("operator_date not between", value1, value2, "operatorDate");
            return (Criteria) this;
        }

        public Criteria andIsProcessedIsNull() {
            addCriterion("is_processed is null");
            return (Criteria) this;
        }

        public Criteria andIsProcessedIsNotNull() {
            addCriterion("is_processed is not null");
            return (Criteria) this;
        }

        public Criteria andIsProcessedEqualTo(Integer value) {
            addCriterion("is_processed =", value, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedNotEqualTo(Integer value) {
            addCriterion("is_processed <>", value, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedGreaterThan(Integer value) {
            addCriterion("is_processed >", value, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedGreaterThanOrEqualTo(Integer value) {
            addCriterion("is_processed >=", value, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedLessThan(Integer value) {
            addCriterion("is_processed <", value, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedLessThanOrEqualTo(Integer value) {
            addCriterion("is_processed <=", value, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedIn(List<Integer> values) {
            addCriterion("is_processed in", values, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedNotIn(List<Integer> values) {
            addCriterion("is_processed not in", values, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedBetween(Integer value1, Integer value2) {
            addCriterion("is_processed between", value1, value2, "isProcessed");
            return (Criteria) this;
        }

        public Criteria andIsProcessedNotBetween(Integer value1, Integer value2) {
            addCriterion("is_processed not between", value1, value2, "isProcessed");
            return (Criteria) this;
        }
    }

    public static class Criteria extends GeneratedCriteria {

        protected Criteria() {
            super();
        }
    }

    public static class Criterion {
        private String condition;

        private Object value;

        private Object secondValue;

        private boolean noValue;

        private boolean singleValue;

        private boolean betweenValue;

        private boolean listValue;

        private String typeHandler;

        public String getCondition() {
            return condition;
        }

        public Object getValue() {
            return value;
        }

        public Object getSecondValue() {
            return secondValue;
        }

        public boolean isNoValue() {
            return noValue;
        }

        public boolean isSingleValue() {
            return singleValue;
        }

        public boolean isBetweenValue() {
            return betweenValue;
        }

        public boolean isListValue() {
            return listValue;
        }

        public String getTypeHandler() {
            return typeHandler;
        }

        protected Criterion(String condition) {
            super();
            this.condition = condition;
            this.typeHandler = null;
            this.noValue = true;
        }

        protected Criterion(String condition, Object value, String typeHandler) {
            super();
            this.condition = condition;
            this.value = value;
            this.typeHandler = typeHandler;
            if (value instanceof List<?>) {
                this.listValue = true;
            } else {
                this.singleValue = true;
            }
        }

        protected Criterion(String condition, Object value) {
            this(condition, value, null);
        }

        protected Criterion(String condition, Object value, Object secondValue, String typeHandler) {
            super();
            this.condition = condition;
            this.value = value;
            this.secondValue = secondValue;
            this.typeHandler = typeHandler;
            this.betweenValue = true;
        }

        protected Criterion(String condition, Object value, Object secondValue) {
            this(condition, value, secondValue, null);
        }
    }
}