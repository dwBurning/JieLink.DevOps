package com.jieshun.devopsserver.bean;

import java.util.ArrayList;
import java.util.List;

public class DevOpsEventFilterExample {
    protected String orderByClause;

    protected boolean distinct;

    protected List<Criteria> oredCriteria;

    public DevOpsEventFilterExample() {
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

        public Criteria andEventCodeIsNull() {
            addCriterion("event_code is null");
            return (Criteria) this;
        }

        public Criteria andEventCodeIsNotNull() {
            addCriterion("event_code is not null");
            return (Criteria) this;
        }

        public Criteria andEventCodeEqualTo(Integer value) {
            addCriterion("event_code =", value, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeNotEqualTo(Integer value) {
            addCriterion("event_code <>", value, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeGreaterThan(Integer value) {
            addCriterion("event_code >", value, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeGreaterThanOrEqualTo(Integer value) {
            addCriterion("event_code >=", value, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeLessThan(Integer value) {
            addCriterion("event_code <", value, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeLessThanOrEqualTo(Integer value) {
            addCriterion("event_code <=", value, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeIn(List<Integer> values) {
            addCriterion("event_code in", values, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeNotIn(List<Integer> values) {
            addCriterion("event_code not in", values, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeBetween(Integer value1, Integer value2) {
            addCriterion("event_code between", value1, value2, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventCodeNotBetween(Integer value1, Integer value2) {
            addCriterion("event_code not between", value1, value2, "eventCode");
            return (Criteria) this;
        }

        public Criteria andEventMessageIsNull() {
            addCriterion("event_message is null");
            return (Criteria) this;
        }

        public Criteria andEventMessageIsNotNull() {
            addCriterion("event_message is not null");
            return (Criteria) this;
        }

        public Criteria andEventMessageEqualTo(String value) {
            addCriterion("event_message =", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageNotEqualTo(String value) {
            addCriterion("event_message <>", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageGreaterThan(String value) {
            addCriterion("event_message >", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageGreaterThanOrEqualTo(String value) {
            addCriterion("event_message >=", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageLessThan(String value) {
            addCriterion("event_message <", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageLessThanOrEqualTo(String value) {
            addCriterion("event_message <=", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageLike(String value) {
            addCriterion("event_message like", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageNotLike(String value) {
            addCriterion("event_message not like", value, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageIn(List<String> values) {
            addCriterion("event_message in", values, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageNotIn(List<String> values) {
            addCriterion("event_message not in", values, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageBetween(String value1, String value2) {
            addCriterion("event_message between", value1, value2, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andEventMessageNotBetween(String value1, String value2) {
            addCriterion("event_message not between", value1, value2, "eventMessage");
            return (Criteria) this;
        }

        public Criteria andIsFilterIsNull() {
            addCriterion("is_filter is null");
            return (Criteria) this;
        }

        public Criteria andIsFilterIsNotNull() {
            addCriterion("is_filter is not null");
            return (Criteria) this;
        }

        public Criteria andIsFilterEqualTo(Integer value) {
            addCriterion("is_filter =", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterNotEqualTo(Integer value) {
            addCriterion("is_filter <>", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterGreaterThan(Integer value) {
            addCriterion("is_filter >", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterGreaterThanOrEqualTo(Integer value) {
            addCriterion("is_filter >=", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterLessThan(Integer value) {
            addCriterion("is_filter <", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterLessThanOrEqualTo(Integer value) {
            addCriterion("is_filter <=", value, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterIn(List<Integer> values) {
            addCriterion("is_filter in", values, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterNotIn(List<Integer> values) {
            addCriterion("is_filter not in", values, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterBetween(Integer value1, Integer value2) {
            addCriterion("is_filter between", value1, value2, "isFilter");
            return (Criteria) this;
        }

        public Criteria andIsFilterNotBetween(Integer value1, Integer value2) {
            addCriterion("is_filter not between", value1, value2, "isFilter");
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